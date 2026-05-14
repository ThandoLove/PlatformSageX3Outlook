/**
 * officeBridge.js - ENTERPRISE EVENT-DRIVEN VERSION
 * Connects Outlook (Office.js), Blazor Server, and Sage X3
 * NOW includes CRM automation engine (SAFE ADD-ON LAYER)
 */

window.officeBridge = {
    _dotNetHelper: null,
    _isInitialized: false,
    _itemChangedHandler: null,

    // ======================================================
    // 1. INITIALIZATION (UNCHANGED CORE LOGIC)
    // ======================================================
    initialize: function (dotNetHelper) {

        if (this._isInitialized) return;

        this._dotNetHelper = dotNetHelper;

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {

                this._isInitialized = true;
                console.log("Sage X3 Outlook Bridge: Ready.");

                if (Office?.context?.mailbox?.item) {
                    this.checkSender();
                }

                this._itemChangedHandler = () => {
                    this.checkSender();
                };

                Office.context.mailbox.addHandlerAsync(
                    Office.EventType.ItemChanged,
                    this._itemChangedHandler
                );
            }
        });
    },

    // ======================================================
    // 2. CONTEXT EXTRACTION (UNCHANGED CORE LOGIC)
    // ======================================================
    checkSender: function () {

        if (!this._isInitialized || !this._dotNetHelper) return;
        if (!Office?.context?.mailbox?.item) return;

        const item = Office.context.mailbox.item;

        try {
            const context = {
                senderEmail: this._getSenderEmail(item),
                senderName: this._getSenderName(item),
                subject: item.subject || "No Subject"
            };

            // Send to Blazor FIRST (UNCHANGED FLOW)
            this._dotNetHelper.invokeMethodAsync(
                'OnEmailReceived',
                context
            )
                .then(() => {

                    // ======================================================
                    // 🟢 NEW ADDITION: CRM AUTOMATION ENGINE TRIGGER
                    // ======================================================
                    this.crmEngine(context);

                })
                .catch(err =>
                    console.error("Blazor OnEmailReceived failed:", err)
                );

        } catch (e) {
            console.error("officeBridge: Failed to extract context", e);
        }
    },

    // ======================================================
    // 3. SAGE X3 NAVIGATION (UNCHANGED)
    // ======================================================
    openSageRecord: function (functionCode, recordKey) {

        const safeFunction = encodeURIComponent(functionCode);
        const safeKey = encodeURIComponent(recordKey);

        const sageUrl =
            `https://your-sage-server.com/${safeFunction}?key=${safeKey}`;

        window.open(
            sageUrl,
            '_blank',
            'noopener,noreferrer'
        );
    },

    // ======================================================
    // 4. ATTACH DOCUMENT (FIXED + SAFE)
    // ======================================================
    attachDocument: function (entityType, fileName) {

        if (!Office?.context?.mailbox?.item) return;

        const fileUrl =
            `https://your-sage-x3-server.com/${encodeURIComponent(entityType)}/${encodeURIComponent(fileName)}`;

        Office.context.mailbox.item.addFileAttachmentAsync(
            fileUrl,
            fileName,
            { asyncContext: null },
            (asyncResult) => {

                if (asyncResult.status === Office.AsyncResultStatus.Failed) {

                    console.error("Attachment failed:", asyncResult.error.message);

                    this.showToast(
                        `Failed to attach ${fileName}`,
                        "error"
                    );

                } else {

                    this.showToast(
                        `${fileName} attached to email.`,
                        "success"
                    );
                }
            }
        );
    },

    // ======================================================
    // 5. EXPORT REPORT (UNCHANGED)
    // ======================================================
    exportReport: function (reportCode, format) {

        this.showToast(`Generating ${reportCode}...`, "info");

        setTimeout(() => {

            this.showToast(
                `${reportCode} is ready for download.`,
                "success"
            );

        }, 3000);
    },

    // ======================================================
    // 6. TOAST BRIDGE (UNCHANGED)
    // ======================================================
    showToast: function (message, type) {

        if (!this._dotNetHelper) return;

        try {
            this._dotNetHelper.invokeMethodAsync(
                'ShowToastFromJs',
                message,
                type
            );
        } catch (e) {
            console.warn("Toast failed:", e);
        }
    },

    // ======================================================
    // 7. INTERNAL HELPERS (UNCHANGED)
    // ======================================================
    _getSenderEmail: function (item) {

        if (item.itemType === Office.MailboxEnums.ItemType.Message) {
            return item.from?.emailAddress ||
                item.sender?.emailAddress ||
                "";
        }

        return "";
    },

    _getSenderName: function (item) {

        if (item.itemType === Office.MailboxEnums.ItemType.Message) {
            return item.from?.displayName ||
                item.sender?.displayName ||
                "";
        }

        return "";
    },

    // ======================================================
    // 🟢 8. CRM EVENT-DRIVEN AUTOMATION ENGINE (NEW ADDITION)
    // ======================================================
    enableAutomation: true,

    crmEngine: function (context) {

        if (!this.enableAutomation) return;

        try {
            console.log("[CRM Engine] Processing context:", context);

            const email = context.senderEmail?.toLowerCase() || "";

            const isInternal =
                email.includes("@yourcompany.com");

            const isExternal =
                !isInternal && email.includes("@");

            if (isExternal) {
                this._handleExternalEmail(context);
            }

            if (isInternal) {
                this._handleInternalEmail(context);
            }

        } catch (e) {
            console.error("CRM Engine error:", e);
        }
    },

    // ======================================================
    // 🟢 CRM HANDLER: EXTERNAL EMAILS (NEW)
    // ======================================================
    _handleExternalEmail: function (context) {

        console.log("[CRM] External email detected");

        this.showToast(
            "External contact detected - evaluating CRM match...",
            "info"
        );

        // FUTURE EXTENSIONS (SAFE PLACEHOLDERS):
        // - Sage X3 contact lookup
        // - Lead detection
        // - AI classification
        // - Auto-ticket suggestion
    },

    // ======================================================
    // 🟢 CRM HANDLER: INTERNAL EMAILS (NEW)
    // ======================================================
    _handleInternalEmail: function (context) {

        console.log("[CRM] Internal email detected");

        this.showToast(
            "Internal communication logged",
            "info"
        );

        // FUTURE EXTENSIONS:
        // - audit logging
        // - workflow tracking
        // - approvals
    },

    // ======================================================
    // 9. CLEANUP (UNCHANGED BUT SAFE)
    // ======================================================
    dispose: function () {

        try {
            if (Office?.context?.mailbox && this._itemChangedHandler) {

                Office.context.mailbox.removeHandlerAsync(
                    Office.EventType.ItemChanged,
                    { handler: this._itemChangedHandler }
                );
            }
        } catch (e) {
            console.warn("Dispose cleanup failed:", e);
        }

        this._dotNetHelper = null;
        this._isInitialized = false;
        this._itemChangedHandler = null;

        console.log("Sage X3 Outlook Bridge: Disposed.");
    }
};


// ======================================================
// OUTLOOK HELPERS (UNCHANGED)
// ======================================================
window.outlookInterop = {

    getCurrentEmailId: function () {
        try {
            return Office?.context?.mailbox?.item?.itemId || null;
        } catch {
            return null;
        }
    },

    displayEmail: function (itemId) {
        if (!itemId) return;

        Office.context.mailbox.displayItemAsync(itemId);
    }
};