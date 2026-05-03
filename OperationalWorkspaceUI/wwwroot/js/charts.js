/**
 * charts.js
 * Handles Chart.js rendering for the Sage X3 Dashboard.
 * Includes "destroy" logic to support real-time data polling.
 */

// Global variables to store chart instances for cleanup
window.sageChartInstance = null;
window.agingChartInstance = null;

/**
 * 1. MAIN DASHBOARD CHART (Sales Orders)
 */
window.renderChart = (id, labels, data) => {
    const ctx = document.getElementById(id);
    if (!ctx) return;

    // LOGIC: Destroy the old chart before creating a new one
    if (window.sageChartInstance) {
        window.sageChartInstance.destroy();
    }

    // CREATE: New Chart with Sage X3 Professional Styling
    window.sageChartInstance = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Sales Orders ($)',
                data: data,
                backgroundColor: '#3b82f6', // Sage Blue
                hoverBackgroundColor: '#003366', // Darker Blue on hover
                borderRadius: 4,
                barThickness: 20
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: '#003366',
                    padding: 10,
                    callbacks: {
                        label: function (context) {
                            return ` Sales: $${context.parsed.y.toLocaleString()}`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: '#f1f5f9' },
                    ticks: { color: '#64748b', font: { size: 11 } }
                },
                x: {
                    grid: { display: false },
                    ticks: { color: '#64748b', font: { size: 11 } }
                }
            }
        }
    });
};

/**
 * 2. TASK AGING HISTOGRAM (Sidebar)
 * Matches the colors from the task mockup (Red, Orange, Yellow, Green)
 */
window.renderAgingHistogram = (id, data) => {
    const ctx = document.getElementById(id);
    if (!ctx) return;

    // Cleanup logic for re-renders
    if (window.agingChartInstance) {
        window.agingChartInstance.destroy();
    }

    window.agingChartInstance = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: {
            labels: ['Overdue', 'Soon', 'Upcoming', 'Completed'],
            datasets: [{
                data: data, // Array of counts from WorkspaceState
                backgroundColor: [
                    '#fca5a5', // Red (Overdue)
                    '#fdba74', // Orange (Due Soon)
                    '#fde68a', // Yellow (Upcoming)
                    '#86efac'  // Green (Completed)
                ],
                borderRadius: 2,
                barThickness: 15
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: { enabled: true }
            },
            scales: {
                y: { display: false, beginAtZero: true },
                x: {
                    grid: { display: false },
                    ticks: { color: '#94a3b8', font: { size: 9, weight: 'bold' } }
                }
            }
        }
    });
};

/**
 * 3. OUTLOOK DEEP LINK
 * Opens the specific email item in Outlook Web
 */
window.openOutlookDeepLink = (id) => {
    if (!id) return;
    // Standard Deep Link format for Outlook Web
    const url = `https://office.com{encodeURIComponent(id)}`;
    window.open(url, '_blank');
};
