// =========================
// 1. CHARTING LOGIC
// =========================

window.dashboardCharts = {
    renderChart: function (canvasId, labels, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (typeof Chart === 'undefined') {
            console.error("Chart.js not found. Include CDN in host file.");
            return;
        }

        new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Sales Data',
                    data: data,
                    backgroundColor: '#005A9E'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }
};


// =========================
// 2. OFFICE.JS BRIDGE (FIXED)
// =========================

window.officeBridge = {
    initialize: function (dotNetHelper) {

        // 🔥 SAFETY: handle browser mode first
        if (typeof Office === 'undefined') {
            console.log("Office.js not available → using polling mode");

            const pollIntervalMs = 2000;
            let lastMessageId = null;

            setInterval(async () => {
                try {
                    const resp = await fetch('/api/testemail/latest');
                    if (!resp.ok) return;

                    const json = await resp.json();
                    if (!json) return;

                    if (json.messageId && json.messageId !== lastMessageId) {
                        lastMessageId = json.messageId;

                        const data = {
                            SenderName: json.senderName || json.sender || 'Test Sender',
                            SenderEmail: json.senderEmail || json.sender || 'test@example.com',
                            Subject: json.subject || 'Test Subject',
                            MessageId: json.messageId || ''
                        };

                        console.log('Polling TestEmail:', data);

                        dotNetHelper.invokeMethodAsync('OnEmailReceived', data);
                    }
                } catch {
                    // silent fail (dev mode)
                }
            }, pollIntervalMs);

            return; // 🔥 IMPORTANT: stop here in browser mode
        }


        // =========================
        // OUTLOOK MODE
        // =========================

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {
                console.log("Office.js Ready. Outlook Host Detected.");

                // 🔥 FIX: preserve context
                const self = this;

                // Initial load
                self.extractEmailData(dotNetHelper);

                // Listen for email changes
                Office.context.mailbox.addHandlerAsync(
                    Office.EventType.ItemChanged,
                    function () {
                        self.extractEmailData(dotNetHelper);
                    }
                );
            }
        });
    },


    extractEmailData: function (dotNetHelper) {

        // 🔥 SAFETY CHECK
        if (typeof Office === 'undefined' || !Office.context || !Office.context.mailbox) {
            return;
        }

        const item = Office.context.mailbox.item;
        if (!item) return;

        try {
            const data = {
                subject: item.subject || "No Subject",
                senderEmail: item.from?.emailAddress || "",
                senderName: item.from?.displayName || "",
                conversationId: item.conversationId || ""
            };

            console.log("Extracting Email Data for Blazor:", data.senderEmail);

            dotNetHelper.invokeMethodAsync('OnEmailReceived', data);

        } catch (e) {
            console.error("extractEmailData failed:", e);
        }
    }
};


// =========================
// 3. UTILITIES
// =========================

window.showToast = function (message) {
    console.log("APP_LOG:", message);
};