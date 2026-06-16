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

                console.log(
                    "Sage X3 Outlook Bridge : Browser mode"
                );

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

            if (
                Office?.context?.mailbox?.item
            ) {

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

                senderEmail:

                    this._getSenderEmail(
                        item
                    ),

                senderName:

                    this._getSenderName(
                        item
                    ),

                subject:

                    item.subject

                    ||

                    "No Subject"
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