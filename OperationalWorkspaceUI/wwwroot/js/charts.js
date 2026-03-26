// CODE START: charts.js

window.renderChart = (id, labels, data) => {
    const ctx = document.getElementById(id).getContext('2d');

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
};

// CODE END