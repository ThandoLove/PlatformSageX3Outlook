/**
 * charts.js
 * Handles Chart.js rendering for the Sage X3 Dashboard.
 * Includes "destroy" logic to support real-time data polling.
 */

// We use a global variable to store the chart instance
window.sageChartInstance = null;

window.renderChart = (id, labels, data) => {
    const ctx = document.getElementById(id);

    if (!ctx) return; // Safety check

    // 1. LOGIC: Destroy the old chart before creating a new one
    // This prevents the "multiple charts on one canvas" flickering bug
    if (window.sageChartInstance) {
        window.sageChartInstance.destroy();
    }

    // 2. CREATE: New Chart with Sage X3 Professional Styling
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
                legend: { display: false }, // Keep it clean like the image
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
