/**
 * officeBridge.js
 * AUTHORITATIVE BASELINE VERSION
 * Outlook + Blazor + Sage X3 Integration Layer
 */

window.officeBridge = {

    // ======================================================
    // RUNTIME STATE
    // ======================================================

    _dotNetHelper: null,
    _isInitialized: false,
    _isInitializing: false,
    _isProcessingEmail: false,
    _itemChangedHandler: null,
    _disposed: false,
    _initRetryCount: 0,
    _maxInitRetries: 5,
    _retryDelayMs: 2000,

    enableAutomation: true,

    // ======================================================
    // INITIALIZATION
    // ======================================================

    initialize: async function (dotNetHelper) {

        if (this._isInitialized || this._isInitializing) {
            return;
        }

        this._isInitializing = true;
        this._dotNetHelper = dotNetHelper;

        try {

            // Browser mode safety
            if (typeof Office === "undefined") {

                console.log("Sage X3 Outlook Bridge : Office.js not found");

                // Retry initialization a few times to allow Office.js to load in hosted environments
                if (this._initRetryCount < this._maxInitRetries) {
                    const delay = this._retryDelayMs * Math.pow(2, this._initRetryCount);
                    console.warn(`officeBridge: Office.js missing, retrying init in ${delay}ms`);
                    this._initRetryCount++;
                    setTimeout(() => {
                        try { this.initialize(dotNetHelper); } catch (e) { console.error('Retry initialize failed', e); }
                    }, delay);
                }

                // In pure browser mode we will not wire Outlook handlers but the bridge still functions in degraded mode
                return;
            }

            await Office.onReady();

            if (
                Office.context.host !==
                Office.HostType.Outlook
            ) {
                console.warn(
                    "Not running inside Outlook"
                );

                return;
            }

            console.log(
                "Sage X3 Outlook Bridge : Initializing"
            );

            // Attach basic window visibility and focus handlers to improve reliability inside Outlook task pane
            try {
                window.addEventListener('visibilitychange', this._onVisibilityChange.bind(this));
                window.addEventListener('focus', this._onWindowFocus.bind(this));
                window.addEventListener('blur', this._onWindowBlur.bind(this));
                window.addEventListener('beforeunload', this._dispose.bind(this));
            }
            catch (e) {
                console.warn('officeBridge: could not attach window event handlers', e);
            }

            this._itemChangedHandler = async () => {

                await this.checkSender();

            };

            Office.context.mailbox.addHandlerAsync(

                Office.EventType.ItemChanged,

                this._itemChangedHandler,

                (result) => {

                    if (
                        result.status ===
                        Office.AsyncResultStatus.Failed
                    ) {

                        console.error(

                            "Failed to register ItemChanged handler:",

                            result.error.message
                        );

                    }
                    else {

                        console.log(
                            "ItemChanged handler registered"
                        );
                    }
                }
            );

            if (Office?.context?.mailbox?.item) {
                await this.checkSender();
            }

            this._isInitialized = true;

            console.log(
                "Sage X3 Outlook Bridge : Ready"
            );

        }
        catch (e) {

            console.error(

                "officeBridge initialize failed:",

                e
            );

        }
        finally {

            this._isInitializing = false;

        }
    },

    // ======================================================
    // LIFECYCLE / WINDOW EVENT HELPERS
    // ======================================================

    _onVisibilityChange: function () {
        try {
            if (document && !document.hidden) {
                // when tab becomes visible, re-check selected item
                if (!this._isProcessingEmail) {
                    this.checkSender();
                }
            }
        }
        catch (e) { console.warn('visibility handler error', e); }
    },

    _onWindowFocus: function () {
        try {
            if (!this._isInitialized && !this._isInitializing) {
                // Try gentle re-init when focus returns
                this.initialize(this._dotNetHelper);
            }
            if (!this._isProcessingEmail) {
                this.checkSender();
            }
        }
        catch (e) { console.warn('focus handler error', e); }
    },

    _onWindowBlur: function () {
        // no-op for now, placeholder for analytics or resource throttling
    },

    _dispose: function () {
        try {
            if (this._disposed) return;
            this._disposed = true;

            // remove mailbox handler if present
            try {
                if (Office?.context?.mailbox && Office.context.mailbox.removeHandlerAsync) {
                    Office.context.mailbox.removeHandlerAsync(Office.EventType.ItemChanged, (res) => { /* noop */ });
                }
            }
            catch (e) { /* ignore */ }

            // remove our window event listeners
            try {
                window.removeEventListener('visibilitychange', this._onVisibilityChange.bind(this));
                window.removeEventListener('focus', this._onWindowFocus.bind(this));
                window.removeEventListener('blur', this._onWindowBlur.bind(this));
                window.removeEventListener('beforeunload', this._dispose.bind(this));
            }
            catch (e) { /* ignore */ }
        }
        catch (e) { console.warn('dispose failed', e); }
    },

    // ======================================================
    // ATTACH DOCUMENT
    // ======================================================

    attachDocument: function (

        urlOrOwner,

        fileName

    ) {

        try {

            console.log(

                "Attach requested:",

                fileName
            );

            let url = null;

            if (

                typeof urlOrOwner === "string"

                &&

                (
                    urlOrOwner.startsWith("/")
                    ||
                    urlOrOwner.startsWith("http")
                )

            ) {

                url = urlOrOwner;

            }
            else {

                url =
                    `/mock-documents/${fileName}`;

            }

            // ====================================
            // Browser mode
            // ====================================

            if (

                typeof Office === "undefined"

                ||

                !Office?.context?.mailbox

            ) {

                console.log(

                    "[Browser Mode] Simulated attach"

                );

                return {

                    success: true,

                    mode: "browser"

                };
            }

            // ====================================
            // Outlook context
            // ====================================

            const item =

                Office?.context?.mailbox?.item;

            // ====================================
            // No email selected
            // ====================================

            if (!item) {

                return {

                    success: false,

                    message: "No email context"

                };
            }

            // ====================================
            // No compose email open
            // ====================================

            if (

                typeof item.addFileAttachmentAsync

                !==

                "function"

            ) {

                console.log(

                    "No compose window detected"

                );

                this.openComposeWindow();

                alert(

                    "A new email was opened.\n\n"

                    +

                    "Click Attach again."

                );

                return {

                    success: true

                };
            }

            // ====================================
            // Attach file
            // ====================================

            item.addFileAttachmentAsync(

                url,

                fileName,

                (result) => {

                    if (

                        result.status ===

                        Office.AsyncResultStatus
                            .Succeeded

                    ) {

                        console.log(

                            "Attachment added"

                        );

                    }
                    else {

                        console.error(

                            "Attachment failed:",

                            result.error.message

                        );
                    }
                }
            );

            return {

                success: true

            };

        }
        catch (e) {

            console.error(

                "attachDocument crash:",

                e
            );

            return {

                success: false,

                message: e.message

            };
        }
    },

    // ======================================================
    // OUTLOOK AVAILABLE
    // ======================================================

    isAvailable: function () {

        try {

            return (

                typeof Office !== "undefined"

                &&

                Office?.context?.mailbox

                &&

                typeof Office
                    .context
                    .mailbox
                    .item

                !==

                "undefined"

            );

        }
        catch {

            return false;

        }
    },

    // ======================================================
    // OPEN NEW COMPOSE
    // ======================================================

    openComposeWindow: function () {

        try {

            if (

                !Office?.context?.mailbox

            ) {

                return false;

            }

            Office.context.mailbox
                .displayNewMessageForm({});

            console.log(

                "Opened compose window"

            );

            return true;

        }
        catch (e) {

            console.error(

                "openComposeWindow failed:",

                e
            );

            return false;

        }
    },

    // ======================================================
    // HAS OPEN DRAFT
    // ======================================================

    hasOpenDraft: function () {

        try {

            const item =

                Office?.context
                    ?.mailbox
                    ?.item;

            if (!item) {

                return false;

            }

            return (

                typeof item
                    .addFileAttachmentAsync

                ===

                "function"

            );

        }
        catch {

            return false;

        }
    },

    // ======================================================
    // UPLOAD CALLBACK
    // ======================================================

    uploadCurrentDocument: function (

        mode

    ) {

        try {

            console.log(

                "Upload:",

                mode
            );

            if (

                this._dotNetHelper

            ) {

                try {

                    this._dotNetHelper
                        .invokeMethodAsync(

                            "OnUploadRequested",

                            mode

                        );

                }
                catch { }
            }

        }
        catch (e) {

            console.error(e);

        }
    },

    openNewEmail: function () {

        this.openComposeWindow();

    },

    // ======================================================
    // EMAIL EXTRACTION
    // ======================================================

    checkSender: async function () {

        if (

            this._isProcessingEmail

        ) {

            return;

        }

        this._isProcessingEmail = true;

        try {

            if (

                !this._dotNetHelper

            ) return;

            if (

                !Office?.context
                    ?.mailbox
                    ?.item

            ) return;

            const item =

                Office.context
                    .mailbox
                    .item;

            const context = {
                // Use Outlook-provided stable identifiers where available
                MessageId: item.internetMessageId || item.itemId || item.conversationId || "",
                SenderEmail: this._getSenderEmail(item),
                SenderName: this._getSenderName(item),
                Subject: item.subject || "No Subject",
                From: (item.from && (item.from.emailAddress || item.from.address)) || (item.sender && (item.sender.emailAddress || item.sender.address)) || "",
                ReceivedAt: (item.dateTimeReceived && item.dateTimeReceived.toISOString && item.dateTimeReceived.toISOString()) || (item.dateTimeCreated && item.dateTimeCreated.toISOString && item.dateTimeCreated.toISOString()) || new Date().toISOString()
            };

            await this._dotNetHelper
                .invokeMethodAsync(

                    "OnEmailReceived",

                    context

                );

            if (

                typeof this.crmEngine

                ===

                "function"

            ) {

                this.crmEngine(

                    context

                );
            }

        }
        catch (e) {

            console.error(

                "checkSender failed:",

                e
            );

        }
        finally {

            this._isProcessingEmail = false;

        }
    },

    // ======================================================
    // SAGE NAVIGATION
    // ======================================================

    openSageRecord: function (

        functionCode,

        action

    ) {

        console.log(

            "Navigating Sage X3:",

            functionCode,

            action
        );

    },

    // ======================================================
    // PRIVATE HELPERS
    // ======================================================

    _getSenderEmail: function (

        item

    ) {

        if (!item) return "";

        if (

            item.itemType ===

            Office.MailboxEnums
                .ItemType
                .Message

            &&

            item.sender

        ) {

            return item.sender
                .emailAddress

                ||

                "";
        }

        if (item.from) {

            return item.from
                .emailAddress

                ||

                "";
        }

        return "";
    },

    _getSenderName: function (

        item

    ) {

        if (!item)

            return "Anonymous";

        if (

            item.itemType ===

            Office.MailboxEnums
                .ItemType
                .Message

            &&

            item.sender

        ) {

            return item.sender
                .displayName

                ||

                "Anonymous";
        }

        if (item.from) {

            return item.from
                .displayName

                ||

                "Anonymous";
        }

        return "Anonymous";
    },

    crmEngine: function (

        context

    ) {

        console.log(

            "CRM Pipeline:",

            context
        );
    }
};