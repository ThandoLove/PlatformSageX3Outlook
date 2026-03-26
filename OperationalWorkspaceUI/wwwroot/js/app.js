// CODE START: app.js

window.dashboardCharts = {
    renderChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Data',
                    data: data
                }]
            }
        });
    }
};

// Optional: toast helper
window.showToast = function (message) {
    console.log("Toast:", message);
};

// CODE END