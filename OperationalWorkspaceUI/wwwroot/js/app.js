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
        // If Office.js is present, wire the normal Outlook handlers.
        if (typeof Office !== 'undefined') {
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
            return;
        }

        // If Office.js is not present, poll our TestEmail API so developers can use Postman to send test emails.
        const pollIntervalMs = 2000; // every 2s
        let lastMessageId = null;
        setInterval(async () => {
            try {
                const resp = await fetch('/api/testemail/latest');
                if (!resp.ok) return;
                const json = await resp.json();
                if (!json) return;
                if (json.messageId && json.messageId !== lastMessageId) {
                    lastMessageId = json.messageId;
                    // Map to the expected shape in MainLayout.OnEmailReceived
                    const data = {
                        SenderName: json.senderName || json.sender || 'Test Sender',
                        SenderEmail: json.senderEmail || json.sender || 'test@example.com',
                        Subject: json.subject || 'Test Subject',
                        MessageId: json.messageId || ''
                    };
                    console.log('Polling TestEmail: ', data);
                    dotNetHelper.invokeMethodAsync('OnEmailReceived', data);
                }
            }
            catch (e) {
                // ignore network errors when backend is not running
            }
        }, pollIntervalMs);
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
