window.renderConsumptionChart = (canvasId, labels, foodData, waterData) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    const existingChart = Chart.getChart(ctx);
    if (existingChart) {
        existingChart.destroy();
    }

    const dpr = window.devicePixelRatio || 1;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Futter (kg)',
                    data: foodData,
                    borderColor: '#f43f5e',
                    backgroundColor: 'rgba(244, 63, 94, 0.1)',
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: '#f43f5e',
                    tension: 0.4,
                    fill: true
                },
                {
                    label: 'Wasser (Liter)',
                    data: waterData,
                    borderColor: '#0ea5e9',
                    backgroundColor: 'rgba(14, 165, 233, 0.1)',
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: '#0ea5e9',
                    tension: 0.4,
                    fill: true
                }
            ]
        },
        options: {
            devicePixelRatio: dpr,
            responsive: true,
            maintainAspectRatio: false,
            interaction: { intersect: false, mode: 'index' },
            plugins: {
                legend: {
                    position: 'top',
                    labels: { usePointStyle: true, font: { family: "'Inter', sans-serif", size: 12, weight: '500' }, padding: 20 }
                },
                tooltip: {
                    backgroundColor: 'rgba(255, 255, 255, 0.9)',
                    titleColor: '#1e293b',
                    bodyColor: '#1e293b',
                    borderColor: '#e2e8f0',
                    borderWidth: 1,
                    padding: 12,
                    boxPadding: 6,
                    usePointStyle: true,
                    titleFont: { weight: 'bold' }
                }
            },
            scales: {
                y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { family: "'Inter', sans-serif" }, color: '#64748b' } },
                x: { grid: { display: false }, ticks: { font: { family: "'Inter', sans-serif" }, color: '#64748b' } }
            }
        }
    });
};

window.renderPerformanceCurve = (canvasId, labels, datasets) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    const existingChart = Chart.getChart(ctx);
    if (existingChart) {
        existingChart.destroy();
    }

    const dpr = window.devicePixelRatio || 1;

    // Farben aus dem Bild nachempfunden
    const colors = {
        layActual: '#0056b3', // Blau
        layNorm: '#e32636',   // Rot
        lossActual: '#000080', // Dunkelblau
        lossNorm: '#ffd700',  // Gelb
        weightActual: '#556b2f', // Dunkelgrün
        weightNorm: '#8b4513'  // Braun
    };

    const formattedDatasets = datasets.map(ds => {
        let borderColor = '#94a3b8';
        let yAxisID = 'y';
        let borderDash = [];
        let pointRadius = 0;

        if (ds.type === 'layActual') borderColor = colors.layActual;
        else if (ds.type === 'layNorm') { borderColor = colors.layNorm; borderDash = [5, 5]; }
        else if (ds.type === 'lossActual') { borderColor = colors.lossActual; yAxisID = 'yLoss'; }
        else if (ds.type === 'lossNorm') { borderColor = colors.lossNorm; yAxisID = 'yLoss'; borderDash = [5, 5]; }
        else if (ds.type === 'weightActual') borderColor = colors.weightActual;
        else if (ds.type === 'weightNorm') { borderColor = colors.weightNorm; borderDash = [5, 5]; }

        return {
            label: ds.label,
            data: ds.data,
            borderColor: borderColor,
            borderWidth: ds.type.includes('Norm') ? 2 : 3,
            borderDash: borderDash,
            pointRadius: ds.type.includes('Actual') ? 2 : 0,
            yAxisID: yAxisID,
            tension: 0.3,
            fill: false
        };
    });

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: formattedDatasets
        },
        options: {
            devicePixelRatio: dpr,
            responsive: true,
            maintainAspectRatio: false,
            interaction: { intersect: false, mode: 'index' },
            plugins: {
                legend: {
                    position: 'top',
                    labels: { boxWidth: 20, font: { size: 11, weight: '500' } }
                }
            },
            scales: {
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    min: 0,
                    max: 100,
                    title: { display: true, text: 'Leistung % / Eigewicht g', font: { weight: 'bold' } },
                    grid: { color: '#f1f5f9' }
                },
                yLoss: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    min: 0,
                    max: 10,
                    title: { display: true, text: 'Verluste %', font: { weight: 'bold' }, color: '#000080' },
                    grid: { drawOnChartArea: false },
                    ticks: { color: '#000080' }
                },
                x: {
                    title: { display: true, text: 'Alter in Wochen', font: { weight: 'bold' } },
                    grid: { color: '#f1f5f9' }
                }
            }
        }
    });
};
