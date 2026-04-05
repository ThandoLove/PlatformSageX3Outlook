// 1. CHARTING LOGIC
window.dashboardCharts = {
    renderChart: function(canvasId, labels, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        // Ensure Chart.js is loaded before calling
        if (typeof Chart === 'undefined') {
            console.error("Chart.js not found. Make sure to include the Chart.js CDN in your _Host.cshtml");
            return;
        }

        new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Sales Data',
                    data: data,
                    backgroundColor: '#005A9E' // Sage/Microsoft Blue
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }
};

// 2. OFFICE.JS BRIDGE (The "Nervous System")
window.officeBridge = {
    initialize: function(dotNetHelper) {
        // Ensure Office.js is actually present
        if (typeof Office === 'undefined') {
            console.warn("Office.js not loaded. This is expected if running outside of Outlook.");
            return;
        }

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {
                console.log("Office.js Ready. Outlook Host Detected.");
                
                // Initial data pull for the currently open email
                this.extractEmailData(dotNetHelper);
                
                // Event Listener: Fires when the user clicks a different email
                Office.context.mailbox.addHandlerAsync(Office.EventType.ItemChanged, () => {
                    this.extractEmailData(dotNetHelper);
                });
            }
        });
    },

    extractEmailData: function(dotNetHelper) {
        const item = Office.context.mailbox.item;
        
        if (item && item.from) {
            const data = {
                subject: item.subject || "No Subject",
                senderEmail: item.from.emailAddress,
                senderName: item.from.displayName,
                conversationId: item.conversationId
            };

            console.log("Extracting Email Data for Blazor:", data.senderEmail);
            
            // Push data to the C# [JSInvokable] method in MainLayout
            dotNetHelper.invokeMethodAsync('OnEmailReceived', data);
        }
    }
};

// 3. UTILITIES
window.showToast = function(message) {
    // This connects to your ToastNotification component logic
    console.log("APP_LOG:", message);
};
