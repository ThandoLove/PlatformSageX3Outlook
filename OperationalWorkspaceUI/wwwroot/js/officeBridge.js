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
        if (this._isInitialized || this._isInitializing) {
            return;
        }

        this._isInitializing = true;
        this._dotNetHelper = dotNetHelper;

        try {
            await Office.onReady();

            if (Office.context.host !== Office.HostType.Outlook) {
                console.warn("Not running inside Outlook");
                return;
            }

            console.log("Sage X3 Outlook Bridge: Initializing");

            this._itemChangedHandler = async () => {
                await this.checkSender();
            };

            Office.context.mailbox.addHandlerAsync(
                Office.EventType.ItemChanged,
                this._itemChangedHandler,
                (result) => {
                    if (result.status === Office.AsyncResultStatus.Failed) {
                        console.error("Failed to register ItemChanged handler:", result.error.message);
                    } else {
                        console.log("ItemChanged handler registered");
                    }
                }
            );

            if (Office?.context?.mailbox?.item) {
                await this.checkSender();
            }

            this._isInitialized = true;
            console.log("Sage X3 Outlook Bridge: Ready");
        } catch (e) {
            console.error("officeBridge initialize failed:", e);
        } finally {
            this._isInitializing = false;
        }
    },

    // ======================================================
    // ATTACH / UPLOAD HELPERS (Used by Blazor UI)
    // ======================================================
    attachDocument: function (urlOrOwner, fileName) {
        try {
            console.log("officeBridge.attachDocument called:", urlOrOwner, fileName);

            // Normalize args: if a URL was provided use it, otherwise build a mock URL
            let url = null;
            if (typeof urlOrOwner === 'string' && (urlOrOwner.startsWith('/') || urlOrOwner.startsWith('http'))) {
                url = urlOrOwner;
            } else if (typeof fileName === 'string') {
                url = `/mock-documents/${fileName}`;
            }

            // If running inside Outlook, attempt to use addFileAttachmentAsync where supported
            if (Office?.context?.mailbox?.item && typeof Office.context.mailbox.item.addFileAttachmentAsync === 'function' && url) {
                Office.context.mailbox.item.addFileAttachmentAsync(url, fileName, function (result) {
                    if (result.status === Office.AsyncResultStatus.Failed) {
                        console.error('addFileAttachmentAsync failed:', result.error && result.error.message);
                    } else {
                        console.log('Attachment added to message via Outlook host');
                    }
                });
                return;
            }

            // Not running in Outlook - simulate a successful attach in development
            console.log('[Mock] attachDocument simulated for', fileName, 'url:', url);
            return;
        }
        catch (e) {
            console.error('officeBridge.attachDocument error', e);
        }
    },

    // Expose a small helper so Blazor can know whether the Outlook host is available
    isAvailable: function () {
        try {
            return (typeof Office !== 'undefined' && Office?.context?.mailbox && typeof Office.context.mailbox.item !== 'undefined');
        } catch (e) {
            return false;
        }
    },

    uploadCurrentDocument: function (mode) {
        try {
            console.log('officeBridge.uploadCurrentDocument called with mode:', mode);

            // If running inside Outlook and host supports file APIs, we could extract the item body or attachments
            // For now, the JS bridge triggers a success path; real implementation would integrate with backend upload endpoints.
            if (this._dotNetHelper) {
                // Notify the .NET layer that an upload was requested (best-effort)
                try { this._dotNetHelper.invokeMethodAsync('OnUploadRequested', mode); } catch (_e) { }
            }

            return;
        } catch (e) {
            console.error('officeBridge.uploadCurrentDocument error', e);
        }
    },

    // ======================================================
    // EMAIL CONTEXT EXTRACTION
    // ======================================================
    checkSender: async function () {
        if (this._isProcessingEmail) {
            return;
        }

        this._isProcessingEmail = true;

        try {
            if (!this._dotNetHelper) return;
            if (!Office?.context?.mailbox?.item) return;

            const item = Office.context.mailbox.item;

            const context = {
                senderEmail: this._getSenderEmail(item),
                senderName: this._getSenderName(item),
                subject: item.subject || "No Subject"
            };

            // Safely push context up into Blazor state management containers
            await this._dotNetHelper.invokeMethodAsync("OnEmailReceived", context);

            if (typeof this.crmEngine === "function") {
                this.crmEngine(context);
            }
        } catch (e) {
            console.error("checkSender failed:", e);
        } finally {
            this._isProcessingEmail = false;
        }
    },

    // ======================================================
    // 🚀 NEW: FIXED SAGE RECORD LINKING PLATFORM
    // ======================================================
    openSageRecord: function (functionCode, action) {
        console.log("Navigating Sage X3:", functionCode, action);

        // This is where you call your external browser redirect 
        // or trigger your underlying Sage Web Services endpoints
        if (Office?.context?.mailbox?.item) {
            const item = Office.context.mailbox.item;
            const email = this._getSenderEmail(item);
            console.log(`Contextual redirect for ${email} with layout ${functionCode}`);
        }
    },

    // ======================================================
    // PRIVATE INTERNAL UTILITIES
    // ======================================================
    _getSenderEmail: function (item) {
        if (!item) return "";
        if (item.itemType === Office.MailboxEnums.ItemType.Message && item.sender) {
            return item.sender.emailAddress || "";
        }
        if (item.from) {
            return item.from.emailAddress || "";
        }
        return "";
    },

    _getSenderName: function (item) {
        if (!item) return "Anonymous";
        if (item.itemType === Office.MailboxEnums.ItemType.Message && item.sender) {
            return item.sender.displayName || "Anonymous";
        }
        if (item.from) {
            return item.from.displayName || "Anonymous";
        }
        return "Anonymous";
    },

    crmEngine: function (context) {
        console.log("CRM Pipeline Processing Email Context:", context);
    }
};
