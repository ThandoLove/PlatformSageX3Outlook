/**
 * officeBridge.js
 * Optimized Bridge between Office.js (Outlook) and Blazor.
 * Handles context extraction and pushes updates to Blazor via DotNetHelper.
 */

window.officeBridge = {
    _dotNetHelper: null,
    _isInitialized: false,

    initialize: function(dotNetHelper) {
        this._dotNetHelper = dotNetHelper;

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {
                this._isInitialized = true;
                console.log("Sage X3 Outlook Bridge: Ready.");

                // 1. Initial check for the currently open email
                this.checkSender();

                // 2. Listen for "ItemChanged" event (user clicks a different email)
                Office.context.mailbox.addHandlerAsync(
                    Office.EventType.ItemChanged,
                    () => { this.checkSender(); }
                );
            }
        });
    },

    checkSender: function() {
        if (!this._isInitialized || !this._dotNetHelper) return;

        const item = Office.context.mailbox.item;
        if (!item) return;

        try {
            // Outlook property paths vary slightly between Read and Compose modes
            // We use fallbacks to ensure we get the data on any platform
            const context = {
                senderEmail: this._getSenderEmail(item),
                senderName: this._getSenderName(item),
                subject: item.subject || "No Subject"
            };

            // Push to Blazor [JSInvokable] HandleEmailSelection
            this._dotNetHelper.invokeMethodAsync('HandleEmailSelection', context)
                .catch(err => console.error("Blazor HandleEmailSelection failed:", err));

        } catch (e) {
            console.error("officeBridge: Failed to extract context", e);
        }
    },

    /**
     * Internal helper to handle different Office.js item shapes for Email
     */
    _getSenderEmail: function(item) {
        if (item.from) {
            return item.from.emailAddress || item.from.address || "";
        }
        // Fallback for different API versions
        return (item.sender && item.sender.emailAddress) ? item.sender.emailAddress : "";
    },

    /**
     * Internal helper to handle different Office.js item shapes for Names
     */
    _getSenderName: function(item) {
        if (item.from) {
            return item.from.displayName || item.from.name || "";
        }
        return (item.sender && item.sender.displayName) ? item.sender.displayName : "";
    }
};

/**
 * Global Utility: Manual trigger for Blazor if needed
 */
window.triggerContextRefresh = function() {
    if (window.officeBridge) {
        window.officeBridge.checkSender();
    }
};
