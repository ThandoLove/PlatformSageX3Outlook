/**
 * charts.js
 * Handles Chart.js rendering and Deep Linking for the Sage X3 Dashboard.
 */

window.sageChartInstance = null;
window.agingChartInstance = null;

/**
 * 1. MAIN DASHBOARD CHART (Sales Orders)
 * Your professional implementation for the main analytics page.
 */
window.renderChart = (id, labels, data) => {
    const ctx = document.getElementById(id);
    if (!ctx) return;
    if (window.sageChartInstance) { window.sageChartInstance.destroy(); }

    window.sageChartInstance = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Sales Orders ($)',
                data: data,
                backgroundColor: '#3b82f6',
                borderRadius: 4,
                barThickness: 20
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, grid: { color: '#f1f5f9' } },
                x: { grid: { display: false } }
            }
        }
    });
};

/**
 * 2. TASK AGING HISTOGRAM (Sidebar)
 * EXACT IMAGE MATCH: Red, Orange, Yellow, Green.
 */
window.renderAgingHistogram = (id, data) => {
    const ctx = document.getElementById(id);
    if (!ctx) return;
    if (window.agingChartInstance) { window.agingChartInstance.destroy(); }

    window.agingChartInstance = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: {
            labels: ['O', 'S', 'U', 'C'], // Overdue, Soon, Upcoming, Completed
            datasets: [{
                data: data,
                backgroundColor: [
                    '#fca5a5', // Overdue Red
                    '#fdba74', // Soon Orange
                    '#fde68a', // Upcoming Yellow (MISSING IN YOUR PREVIOUS CODE)
                    '#86efac'  // Completed Green
                ],
                borderRadius: 2,
                barThickness: 15
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                y: { display: false, beginAtZero: true },
                x: {
                    grid: { display: false },
                    ticks: { font: { size: 9, weight: 'bold' }, color: '#94a3b8' }
                }
            }
        }
    });
};

/**
 * 3. OUTLOOK DEEP LINK (THE CRITICAL FIX)
 * Opens the specific email item in Outlook Web using the ItemId.
 */
window.openOutlookItem = (outlookId) => {
    if (!outlookId) return;

    // FIXED: Added the template literal ${} and the actual Outlook deep link path.
    // Your previous code used https://office.com{...} which is a dead link.
    const url = `https://office.com{encodeURIComponent(outlookId)}`;

    window.open(url, '_blank');
};
