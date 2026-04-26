/**
 * officeBridge.js - FINAL CONSOLIDATED VERSION
 * Connects Outlook (Office.js), Blazor Server, and Sage X3.
 */

window.officeBridge = {
    _dotNetHelper: null,
    _isInitialized: false,

    // 1. INITIALIZATION: Links Blazor and Outlook
    initialize: function (dotNetHelper) {
        this._dotNetHelper = dotNetHelper;

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {
                this._isInitialized = true;
                console.log("Sage X3 Outlook Bridge: Ready.");

                // Initial context check
                this.checkSender();

                // Listen for user clicking different emails
                Office.context.mailbox.addHandlerAsync(
                    Office.EventType.ItemChanged,
                    () => { this.checkSender(); }
                );
            }
        });
    },

    // 2. CONTEXT EXTRACTION: Sends Email data to Blazor
    checkSender: function () {
        if (!this._isInitialized || !this._dotNetHelper) return;

        const item = Office.context.mailbox.item;
        if (!item) return;

        try {
            const context = {
                senderEmail: this._getSenderEmail(item),
                senderName: this._getSenderName(item),
                subject: item.subject || "No Subject"
            };

            // Push to Blazor [JSInvokable] in MainLayout or RightPanel
            this._dotNetHelper.invokeMethodAsync('OnEmailReceived', context)
                .catch(err => console.error("Blazor OnEmailReceived failed:", err));

        } catch (e) {
            console.error("officeBridge: Failed to extract context", e);
        }
    },

    // 3. ACTION: OPEN SAGE X3 RECORDS
    openSageRecord: function (functionCode, recordKey) {
        console.log(`[Sage X3] Navigating to ${functionCode} for ID: ${recordKey}`);
        // In a real Syracuse environment, this URL structure is used:
        const sageUrl = `https://your-sage-server.com{functionCode}.$query&where=key%20eq%20'${recordKey}'`;

        // Opens Sage X3 in a new tab or the Syracuse frame
        window.open(sageUrl, '_blank');
    },

    // 4. ACTION: ATCH DOCUMENT TO OUTLOOK EMAIL
    attachDocument: function (entityType, fileName) {
        console.log(`[Sage Bridge] Attempting to attach: ${fileName}`);

        // Construct the source URL from Sage X3 storage
        const fileUrl = `https://your-sage-x3-server.com{entityType}/${fileName}`;

        // Inject file directly into the active Outlook Compose window
        Office.context.mailbox.item.addFileAttachmentAsync(
            fileUrl,
            fileName,
            { asyncContext: null },
            (asyncResult) => {
                if (asyncResult.status === Office.AsyncResultStatus.Failed) {
                    console.error("Attachment failed: " + asyncResult.error.message);
                    this.showToast(`Failed to attach ${fileName}`, "error");
                } else {
                    this.showToast(`${fileName} attached to email.`, "success");
                }
            }
        );
    },

    // 5. ACTION: EXPORT SAGE REPORTS
    exportReport: function (reportCode, format) {
        console.log(`[Sage X3] Generating ${format} for ${reportCode}`);
        this.showToast(`Generating ${reportCode}...`, "info");

        // Simulate background process for testing
        setTimeout(() => {
            this.showToast(`${reportCode} is ready for download.`, "success");
        }, 3000);
    },

    // 6. UI UTILITY: Pass messages back to Blazor ToastService
    showToast: function (message, type) {
        if (this._dotNetHelper) {
            // Invokes the Toast logic in your Blazor app
            this._dotNetHelper.invokeMethodAsync('ShowToastFromJs', message, type);
        }
    },

    // INTERNAL HELPERS
    _getSenderEmail: function (item) {
        if (item.itemType === Office.MailboxEnums.ItemType.Message) {
            return (item.from) ? (item.from.emailAddress || item.from.address) : (item.sender ? item.sender.emailAddress : "");
        }
        return "";
    },

    _getSenderName: function (item) {
        if (item.itemType === Office.MailboxEnums.ItemType.Message) {
            return (item.from) ? (item.from.displayName || item.from.name) : (item.sender ? item.sender.displayName : "");
        }
        return "";
    }
};
