/**
 * officeBridge.js
 * ENTERPRISE HARDENED VERSION
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

        // ======================================================
        // PREVENT DUPLICATE INITIALIZATION
        // ======================================================

        if (this._isInitialized || this._isInitializing) {
            return;
        }

        this._isInitializing = true;

        this._dotNetHelper = dotNetHelper;

        try {

            // ======================================================
            // WAIT FOR OFFICE
            // ======================================================

            await Office.onReady();

            if (Office.context.host !== Office.HostType.Outlook) {
                console.warn("Not running inside Outlook");
                return;
            }

            console.log(
                "Sage X3 Outlook Bridge: Initializing");

            // ======================================================
            // ITEM CHANGED HANDLER
            // ======================================================

            this._itemChangedHandler =
                async () => {

                    await this.checkSender();
                };

            // ======================================================
            // REGISTER OUTLOOK EVENT
            // ======================================================

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
                            "ItemChanged handler registered");
                    }
                });

            // ======================================================
            // INITIAL EMAIL LOAD
            // ======================================================

            if (Office?.context?.mailbox?.item) {

                await this.checkSender();
            }

            this._isInitialized = true;

            console.log(
                "Sage X3 Outlook Bridge: Ready");
        }
        catch (e) {

            console.error(
                "officeBridge initialize failed:",
                e);
        }
        finally {

            this._isInitializing = false;
        }
    },

    // ======================================================
    // EMAIL CONTEXT EXTRACTION
    // ======================================================

    checkSender: async function () {

        // ======================================================
        // PREVENT OVERLAPPING EXECUTION
        // ======================================================

        if (this._isProcessingEmail) {
            return;
        }

        this._isProcessingEmail = true;

        try {

            if (!this._dotNetHelper) {
                return;
            }

            if (!Office?.context?.mailbox?.item) {
                return;
            }

            const item =
                Office.context.mailbox.item;

            const context = {

                senderEmail:
                    this._getSenderEmail(item),

                senderName:
                    this._getSenderName(item),

                subject:
                    item.subject || "No Subject"
            };

            // ======================================================
            // SEND TO BLAZOR
            // ======================================================

            await this._dotNetHelper
                .invokeMethodAsync(
                    "OnEmailReceived",
                    context
                );

            // ======================================================
            // CRM ENGINE
            // ======================================================

            this.crmEngine(context);
        }
        catch (e) {

            console.error(
                "checkSender failed:",
                e);
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
        recordKey
    ) {

        const safeFunction =
            encodeURIComponent(functionCode);

        const safeKey =
            encodeURIComponent(recordKey);

        const sageUrl =
            `https://your-sage-server.com/${safeFunction}?key=${safeKey}`;

        window.open(
            sageUrl,
            "_blank",
            "noopener,noreferrer"
        );
    },

    // ======================================================
    // DOCUMENT ATTACHMENT
    // ======================================================

    attachDocument: function (
        entityType,
        fileName
    ) {

        if (!Office?.context?.mailbox?.item) {
            return;
        }

        const fileUrl =
            `https://your-sage-x3-server.com/${encodeURIComponent(entityType)}/${encodeURIComponent(fileName)}`;

        Office.context.mailbox.item
            .addFileAttachmentAsync(
                fileUrl,
                fileName,
                { asyncContext: null },
                (asyncResult) => {

                    if (
                        asyncResult.status ===
                        Office.AsyncResultStatus.Failed
                    ) {

                        console.error(
                            "Attachment failed:",
                            asyncResult.error.message
                        );

                        this.showToast(
                            `Failed to attach ${fileName}`,
                            "error"
                        );
                    }
                    else {

                        this.showToast(
                            `${fileName} attached successfully`,
                            "success"
                        );
                    }
                });
    },

    // ======================================================
    // REPORT EXPORT
    // ======================================================

    exportReport: function (
        reportCode,
        format
    ) {

        this.showToast(
            `Generating ${reportCode}...`,
            "info"
        );

        setTimeout(() => {

            this.showToast(
                `${reportCode} ready for download`,
                "success"
            );

        }, 3000);
    },

    // ======================================================
    // TOAST BRIDGE
    // ======================================================

    showToast: function (
        message,
        type
    ) {

        if (!this._dotNetHelper) {
            return;
        }

        try {

            this._dotNetHelper
                .invokeMethodAsync(
                    "ShowToastFromJs",
                    message,
                    type
                );
        }
        catch (e) {

            console.warn(
                "Toast bridge failed:",
                e);
        }
    },

    // ======================================================
    // HELPERS
    // ======================================================

    _getSenderEmail: function (item) {

        if (
            item.itemType ===
            Office.MailboxEnums.ItemType.Message
        ) {

            return (
                item.from?.emailAddress ||
                item.sender?.emailAddress ||
                ""
            );
        }

        return "";
    },

    _getSenderName: function (item) {

        if (
            item.itemType ===
            Office.MailboxEnums.ItemType.Message
        ) {

            return (
                item.from?.displayName ||
                item.sender?.displayName ||
                ""
            );
        }

        return "";
    },

    // ======================================================
    // CRM AUTOMATION ENGINE
    // ======================================================

    crmEngine: function (context) {

        if (!this.enableAutomation) {
            return;
        }

        try {

            console.log(
                "[CRM Engine] Processing:",
                context);

            const email =
                context.senderEmail
                    ?.toLowerCase() || "";

            const isInternal =
                email.includes("@yourcompany.com");

            const isExternal =
                !isInternal &&
                email.includes("@");

            if (isExternal) {

                this._handleExternalEmail(
                    context);
            }

            if (isInternal) {

                this._handleInternalEmail(
                    context);
            }
        }
        catch (e) {

            console.error(
                "CRM Engine failed:",
                e);
        }
    },

    // ======================================================
    // EXTERNAL EMAIL HANDLER
    // ======================================================

    _handleExternalEmail: function (
        context
    ) {

        console.log(
            "[CRM] External email detected");

        this.showToast(
            "External contact detected",
            "info"
        );
    },

    // ======================================================
    // INTERNAL EMAIL HANDLER
    // ======================================================

    _handleInternalEmail: function (
        context
    ) {

        console.log(
            "[CRM] Internal email detected");

        this.showToast(
            "Internal communication logged",
            "info"
        );
    },

    // ======================================================
    // CLEANUP
    // ======================================================

    dispose: function () {

        try {

            // ======================================================
            // REMOVE OUTLOOK EVENT HANDLER
            // ======================================================

            if (
                Office?.context?.mailbox &&
                this._itemChangedHandler
            ) {

                Office.context.mailbox
                    .removeHandlerAsync(
                        Office.EventType.ItemChanged,
                        {
                            handler:
                                this._itemChangedHandler
                        },
                        (result) => {

                            if (
                                result.status ===
                                Office.AsyncResultStatus.Failed
                            ) {

                                console.warn(
                                    "Failed removing handler:",
                                    result.error.message
                                );
                            }
                            else {

                                console.log(
                                    "ItemChanged handler removed");
                            }
                        });
            }
        }
        catch (e) {

            console.warn(
                "Office cleanup failed:",
                e);
        }

        // ======================================================
        // RELEASE DOTNET REFERENCE
        // ======================================================

        try {

            if (this._dotNetHelper) {

                this._dotNetHelper.dispose?.();
            }
        }
        catch (e) {

            console.warn(
                "DotNet cleanup failed:",
                e);
        }

        // ======================================================
        // RESET RUNTIME STATE
        // ======================================================

        this._dotNetHelper = null;

        this._isInitialized = false;

        this._isInitializing = false;

        this._isProcessingEmail = false;

        this._itemChangedHandler = null;

        console.log(
            "Sage X3 Outlook Bridge: Disposed");
    }
};

// ======================================================
// OUTLOOK HELPERS
// ======================================================

window.outlookInterop = {

    getCurrentEmailId: function () {

        try {

            return (
                Office?.context
                    ?.mailbox
                    ?.item
                    ?.itemId || null
            );
        }
        catch {

            return null;
        }
    },

    displayEmail: function (itemId) {

        if (!itemId) {
            return;
        }

        Office.context.mailbox
            .displayItemAsync(itemId);
    }
};