/**
 * charts.js (SAFE VERSION)
 * Bulletproof Chart.js integration for Blazor Server
 * - Prevents JSInterop crashes
 * - Handles missing dependencies
 * - Cleans up chart instances properly
 */

(function () {

    // GLOBAL INSTANCES
    window.sageChartInstance = null;
    window.agingChartInstance = null;

    /**
     * SAFE HELPER: Ensures Chart.js exists
     */
    function isChartAvailable() {
        if (typeof Chart === "undefined") {
            console.warn("Chart.js is NOT loaded.");
            return false;
        }
        return true;
    }

    /**
     * SAFE HELPER: Get canvas context
     */
    function getContext(id) {
        const canvas = document.getElementById(id);

        if (!canvas) {
            console.warn(`Canvas not found: ${id}`);
            return null;
        }

        return canvas.getContext("2d");
    }

    /**
     * DESTROY helper (prevents memory leaks)
     */
    function destroyChart(instance) {
        try {
            if (instance) {
                instance.destroy();
            }
        } catch (e) {
            console.warn("Chart destroy failed:", e);
        }
    }

    /**
     * 1. MAIN DASHBOARD CHART (SAFE)
     */
    window.renderChart = function (id, labels, data) {
        try {
            if (!isChartAvailable()) return;

            const ctx = getContext(id);
            if (!ctx) return;

            destroyChart(window.sageChartInstance);

            window.sageChartInstance = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels || [],
                    datasets: [{
                        label: 'Sales Orders ($)',
                        data: data || [],
                        backgroundColor: '#3b82f6',
                        borderRadius: 4,
                        barThickness: 20
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: '#f1f5f9' }
                        },
                        x: {
                            grid: { display: false }
                        }
                    }
                }
            });

        } catch (err) {
            console.error("renderChart failed:", err);
        }
    };

    /**
     * 2. TASK AGING HISTOGRAM (SAFE)
     */
    window.renderAgingHistogram = function (id, data) {
        try {
            if (!isChartAvailable()) return;

            const ctx = getContext(id);
            if (!ctx) return;

            destroyChart(window.agingChartInstance);

            window.agingChartInstance = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ['O', 'S', 'U', 'C'], // Overdue, Soon, Upcoming, Completed
                    datasets: [{
                        data: data || [0, 0, 0, 0],
                        backgroundColor: [
                            '#fca5a5', // Overdue
                            '#fdba74', // Soon
                            '#fde68a', // Upcoming
                            '#86efac'  // Completed
                        ],
                        borderRadius: 2,
                        barThickness: 15
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false }
                    },
                    scales: {
                        y: {
                            display: false,
                            beginAtZero: true
                        },
                        x: {
                            grid: { display: false },
                            ticks: {
                                font: { size: 9, weight: 'bold' },
                                color: '#94a3b8'
                            }
                        }
                    }
                }
            });

        } catch (err) {
            console.error("renderAgingHistogram failed:", err);
        }
    };

    /**
     * 3. OUTLOOK DEEP LINK (FIXED)
     */
    window.openOutlookItem = function (outlookId) {
        try {
            if (!outlookId) {
                console.warn("No Outlook ID provided.");
                return;
            }

            // ✅ CORRECT URL
            const url = `https://outlook.office.com/mail/inbox/id/${encodeURIComponent(outlookId)}`;

            window.open(url, '_blank');

        } catch (err) {
            console.error("openOutlookItem failed:", err);
        }
    };

})();