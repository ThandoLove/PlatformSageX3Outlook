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
// 2. OFFICE.JS BRIDGE (FIXED & ALIGNED)
// =========================

window.officeBridge = {
    initialize: function (dotNetHelper) {

        // 🔥 SAFETY: handle browser mode first
        if (typeof Office === 'undefined') {
            console.log("Office.js not available → running browser mode (Single Fetch)");

            // 🔥 FIX: Fetches ONCE on application load instead of looping on a 2-second timer
            (async () => {
                try {
                    const resp = await fetch('https://localhost:7123/api/testemail/latest');
                    if (!resp.ok) return;

                    const json = await resp.json();
                    if (!json) return;

                    // 🔥 FIX: Aligned properties directly with PascalCase EmailInsightDto naming
                    const data = {
                        SenderName: json.senderName || json.sender || 'Test Sender',
                        SenderEmail: json.senderEmail || json.sender || 'test@example.com',
                        Subject: json.subject || 'Test Subject',
                        MessageId: json.messageId || 'MOCK_SAGE_X3_TEST_CONVERSATION',
                        From: json.senderEmail || json.sender || 'test@example.com'
                    };

                    console.log('Single Polling TestEmail Loaded:', data);

                    dotNetHelper.invokeMethodAsync('OnEmailReceived', data);
                } catch {
                    // silent fail (dev mode)
                }
            })();

            return; // 🔥 IMPORTANT: stop here in browser mode
        }


        // =========================
        // OUTLOOK MODE
        // =========================

        Office.onReady((info) => {
            if (info.host === Office.HostType.Outlook) {
                console.log("Office.js Ready. Outlook Host Detected.");

                // 🔥 FIX: Using arrow functions preserves 'this' context perfectly
                // Initial load
                this.extractEmailData(dotNetHelper);

                // Listen for email changes
                Office.context.mailbox.addHandlerAsync(
                    Office.EventType.ItemChanged,
                    () => {
                        this.extractEmailData(dotNetHelper);
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
            // 🔥 FIX: Standardised to PascalCase keys to prevent payload mapping dropping out
            const data = {
                Subject: item.subject || "No Subject",
                SenderEmail: item.from?.emailAddress || "",
                SenderName: item.from?.displayName || "",
                MessageId: item.conversationId || "", // Maps conversationId directly to MessageId container
                From: item.from?.emailAddress || ""
            };

            console.log("Extracting Email Data for Blazor:", data.SenderEmail);

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


// =========================
// 4. INTEROP SHIMS (safety stubs for missing integrations)
// These provide no-op or demo behaviors when host integrations are not present
// so Blazor JS interop calls won't throw "not a function" errors during demo.
// =========================

window.openOutlookItem = function (itemId) {
    try {
        console.log("openOutlookItem called:", itemId);
        if (itemId && itemId.startsWith && itemId.startsWith("http")) {
            window.open(itemId, "_blank");
        } else {
            window.open('/compose.html?source=' + encodeURIComponent(itemId || ''), '_blank');
        }
    } catch (e) {
        console.error('openOutlookItem failed', e);
    }
};

window.openSageX3CustomerContext = function (customerName) {
    try {
        console.log('openSageX3CustomerContext called for:', customerName);
        var url = '/sagex3/customer?name=' + encodeURIComponent(customerName || '');
        window.open(url, '_blank');
    } catch (e) {
        console.error('openSageX3CustomerContext failed', e);
    }
};

window.renderAgingHistogram = function (canvasId, dataArray) {
    try {
        console.log('renderAgingHistogram', canvasId, dataArray);
        var canvas = document.getElementById(canvasId);
        if (!canvas) return;
        var ctx = canvas.getContext('2d');
        if (!ctx) return;
        var w = canvas.width = canvas.clientWidth || 200;
        var h = canvas.height = canvas.clientHeight || 60;
        ctx.clearRect(0, 0, w, h);
        var max = Math.max.apply(null, dataArray || [0]);
        var barWidth = Math.floor(w / ((dataArray && dataArray.length) || 1)) - 4;
        for (var i = 0; i < (dataArray ? dataArray.length : 0); i++) {
            var v = dataArray[i] || 0;
            var barH = max > 0 ? (v / max) * (h - 8) : 0;
            var x = i * (barWidth + 4) + 4;
            var y = h - barH - 4;
            ctx.fillStyle = '#3b82f6';
            ctx.fillRect(x, y, barWidth, barH);
        }
    } catch (e) {
        console.error('renderAgingHistogram failed', e);
    }
};
