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

    // Moderne, kräftige, aber edle Farben
    const colors = {
        layActual: '#3b82f6', // Blau (kräftig)
        layNorm: 'rgba(59, 130, 246, 0.5)',   // Blau (weich für Norm)
        lossActual: '#ef4444', // Rot
        lossNorm: 'rgba(239, 68, 68, 0.5)',  // Rot (weich)
        weightActual: '#10b981', // Smaragd-Grün
        weightNorm: 'rgba(16, 185, 129, 0.5)'  // Smaragd (weich)
    };

    const formattedDatasets = datasets.map(ds => {
        let borderColor = '#94a3b8';
        let yAxisID = 'y';
        let borderDash = [];
        let pointRadius = 0;
        let isNorm = ds.type.includes('Norm');

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
            borderWidth: isNorm ? 2 : 3,
            borderDash: borderDash,
            pointRadius: isNorm ? 0 : 3,
            pointBackgroundColor: borderColor,
            pointBorderColor: '#ffffff',
            pointBorderWidth: 1.5,
            pointHoverRadius: 6,
            yAxisID: yAxisID,
            tension: 0.4, // Weiche Kurven
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
                    labels: { usePointStyle: true, font: { family: "'Inter', sans-serif", size: 12, weight: '500' }, padding: 20 }
                },
                tooltip: {
                    backgroundColor: 'rgba(255, 255, 255, 0.95)',
                    titleColor: '#0f172a',
                    bodyColor: '#334155',
                    borderColor: '#e2e8f0',
                    borderWidth: 1,
                    padding: 12,
                    boxPadding: 6,
                    usePointStyle: true,
                    titleFont: { family: "'Inter', sans-serif", size: 13, weight: 'bold' },
                    bodyFont: { family: "'Inter', sans-serif", size: 12, weight: '500' },
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                label += context.parsed.y;
                                if (label.includes('Leistung') || label.includes('Verlust')) label += ' %';
                                if (label.includes('gewicht')) label += ' g';
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    min: 0,
                    max: 100,
                    title: { display: true, text: 'Leistung % / Eigewicht g', font: { family: "'Inter', sans-serif", weight: 'bold', size: 12 }, color: '#475569' },
                    grid: { color: '#f1f5f9' },
                    ticks: { font: { family: "'Inter', sans-serif" }, color: '#64748b' }
                },
                yLoss: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    min: 0,
                    max: 10,
                    title: { display: true, text: 'Verluste %', font: { family: "'Inter', sans-serif", weight: 'bold', size: 12 }, color: colors.lossActual },
                    grid: { drawOnChartArea: false },
                    ticks: { font: { family: "'Inter', sans-serif" }, color: colors.lossActual }
                },
                x: {
                    title: { display: true, text: 'Alter in Wochen', font: { family: "'Inter', sans-serif", weight: 'bold', size: 12 }, color: '#475569' },
                    grid: { color: '#f1f5f9' },
                    ticks: { font: { family: "'Inter', sans-serif" }, color: '#64748b' }
                }
            }
        }
    });
};
