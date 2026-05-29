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
