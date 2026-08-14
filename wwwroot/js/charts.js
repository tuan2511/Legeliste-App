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
    const canvasContext = ctx.getContext('2d');

    // Gradienten für die Flächen unter den Kurven
    let layGradient = canvasContext.createLinearGradient(0, 0, 0, 300);
    layGradient.addColorStop(0, 'rgba(79, 70, 229, 0.4)'); // Deep Indigo
    layGradient.addColorStop(1, 'rgba(79, 70, 229, 0.0)');

    let lossGradient = canvasContext.createLinearGradient(0, 0, 0, 300);
    lossGradient.addColorStop(0, 'rgba(20, 184, 166, 0.4)'); // Teal
    lossGradient.addColorStop(1, 'rgba(20, 184, 166, 0.0)');

    let weightGradient = canvasContext.createLinearGradient(0, 0, 0, 300);
    weightGradient.addColorStop(0, 'rgba(139, 92, 246, 0.4)'); // Violet
    weightGradient.addColorStop(1, 'rgba(139, 92, 246, 0.0)');

    // Moderne, eigenständige Farben
    const colors = {
        layActual: '#4f46e5', // Deep Indigo
        lossActual: '#14b8a6', // Teal
        weightActual: '#8b5cf6', // Violet
        norm: '#cbd5e1' // Zurückhaltendes Grau
    };

    // Fokus-Modus: Finde den höchsten Index mit echten Daten
    let maxActualIndex = 0;
    datasets.forEach(ds => {
        if (!ds.type.includes('Norm')) {
            for (let i = ds.data.length - 1; i >= 0; i--) {
                if (ds.data[i] !== null && ds.data[i] !== undefined) {
                    maxActualIndex = Math.max(maxActualIndex, i);
                    break;
                }
            }
        }
    });
    // +4 Wochen Puffer in die Zukunft, aber maximal bis zum Ende der Labels
    let maxXLimitIndex = Math.min(labels.length - 1, maxActualIndex + 4);

    const formattedDatasets = datasets.map(ds => {
        let borderColor = '#94a3b8';
        let backgroundColor = 'transparent';
        let yAxisID = 'y';
        let borderDash = [];
        let isNorm = ds.type.includes('Norm');
        let fill = false;

        // "unter der Haupt-Leistungskurve einen leichten, halbtransparenten Farbverlauf"
        // Wir können es auf alle anwenden oder primär auf die Hauptkurven.
        if (ds.type === 'layActual') { borderColor = colors.layActual; backgroundColor = layGradient; fill = true; }
        else if (ds.type === 'layNorm') { borderColor = colors.norm; borderDash = [5, 5]; }
        else if (ds.type === 'lossActual') { borderColor = colors.lossActual; yAxisID = 'yLoss'; backgroundColor = lossGradient; fill = true; }
        else if (ds.type === 'lossNorm') { borderColor = colors.norm; yAxisID = 'yLoss'; borderDash = [5, 5]; }
        else if (ds.type === 'weightActual') { borderColor = colors.weightActual; backgroundColor = weightGradient; fill = true; }
        else if (ds.type === 'weightNorm') { borderColor = colors.norm; borderDash = [5, 5]; }

        return {
            label: ds.label,
            data: ds.data,
            borderColor: borderColor,
            backgroundColor: backgroundColor,
            borderWidth: isNorm ? 2 : 4,
            borderDash: borderDash,
            pointRadius: isNorm ? 0 : 6,
            pointBackgroundColor: borderColor,
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2,
            pointHoverRadius: 8,
            yAxisID: yAxisID,
            tension: 0.4, // Weiche, abgerundete Kurven (Bezier)
            fill: fill
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
                    labels: { 
                        usePointStyle: true, 
                        boxWidth: 8,
                        boxHeight: 8,
                        font: { family: "'Inter', sans-serif", size: 12, weight: '500' }, 
                        padding: 24,
                        color: '#64748b'
                    }
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
                        title: function(context) {
                            return 'Woche ' + context[0].label;
                        },
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
                    title: { display: false }, // Minimalistisch
                    grid: { color: '#f8fafc' }, // Nur helle horizontale Hilfslinien
                    border: { display: false },
                    ticks: { font: { family: "'Inter', sans-serif", size: 11 }, color: '#94a3b8', padding: 8 }
                },
                yLoss: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    min: 0,
                    max: 10,
                    title: { display: false },
                    grid: { display: false },
                    border: { display: false },
                    ticks: { font: { family: "'Inter', sans-serif", size: 11 }, color: '#94a3b8', padding: 8 }
                },
                x: {
                    max: labels[maxXLimitIndex],
                    title: { display: false },
                    grid: { display: false }, // Kein vertikales Raster
                    border: { display: false },
                    ticks: { font: { family: "'Inter', sans-serif", size: 11 }, color: '#94a3b8', padding: 8, maxTicksLimit: 12 }
                }
            }
        }
    });
};
