"use strict";
// ─────────────────────────────────────────────────────────────
//  yieldChart.ts  –  Styled gradient line chart for Crop Yield
//  Compiled to wwwroot/js/yieldChart.js via TypeScript
// ─────────────────────────────────────────────────────────────
// Vibrant palette with gradient fill colors
const cropColors = [
    { border: '#f59e0b', bgStart: 'rgba(245,158,11,0.35)', bgEnd: 'rgba(245,158,11,0.02)' },
    { border: '#22c55e', bgStart: 'rgba(34,197,94,0.35)', bgEnd: 'rgba(34,197,94,0.02)' },
    { border: '#60a5fa', bgStart: 'rgba(96,165,250,0.35)', bgEnd: 'rgba(96,165,250,0.02)' },
    { border: '#a78bfa', bgStart: 'rgba(167,139,250,0.35)', bgEnd: 'rgba(167,139,250,0.02)' },
    { border: '#f87171', bgStart: 'rgba(248,113,113,0.35)', bgEnd: 'rgba(248,113,113,0.02)' },
    { border: '#34d399', bgStart: 'rgba(52,211,153,0.35)', bgEnd: 'rgba(52,211,153,0.02)' },
    { border: '#fb923c', bgStart: 'rgba(251,146,60,0.35)', bgEnd: 'rgba(251,146,60,0.02)' },
    { border: '#38bdf8', bgStart: 'rgba(56,189,248,0.35)', bgEnd: 'rgba(56,189,248,0.02)' },
];
/**
 * Create a vertical gradient for the fill under each line.
 */
function createGradient(ctx, colorStart, colorEnd) {
    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, colorStart);
    gradient.addColorStop(1, colorEnd);
    return gradient;
}
/**
 * Render a styled gradient line chart for crop yield analytics.
 * @param canvasId - The id of the target <canvas> element.
 * @param records  - Array of harvest records serialized from the server.
 */
function renderYieldChart(canvasId, records) {
    const canvas = document.getElementById(canvasId);
    if (!canvas || records.length === 0)
        return;
    const ctx = canvas.getContext('2d');
    if (!ctx)
        return;
    // Build x-axis labels (unique dates in order)
    const allDates = [...new Set(records.map((r) => r.date))];
    // Build one dataset per crop
    const crops = [...new Set(records.map((r) => r.crop))];
    const datasets = crops.map((cropName, i) => {
        const color = cropColors[i % cropColors.length];
        const data = allDates.map((date) => {
            const match = records.find((r) => r.crop === cropName && r.date === date);
            return match ? Number(match.yield) : null;
        });
        return {
            label: cropName,
            data: data,
            borderColor: color.border,
            backgroundColor: createGradient(ctx, color.bgStart, color.bgEnd),
            pointBackgroundColor: color.border,
            pointBorderColor: '#0d1b2a',
            pointBorderWidth: 2,
            pointRadius: 5,
            pointHoverRadius: 8,
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: color.border,
            pointHoverBorderWidth: 3,
            borderWidth: 2.5,
            tension: 0.4,
            spanGaps: true,
            fill: true
        };
    });
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: allDates,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            interaction: { mode: 'index', intersect: false },
            hover: { mode: 'index', intersect: false },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: '#cbd5e1',
                        font: { size: 12, weight: '500', family: "'Segoe UI', system-ui, sans-serif" },
                        usePointStyle: true,
                        pointStyle: 'circle',
                        pointStyleWidth: 10,
                        padding: 24,
                        boxHeight: 8
                    }
                },
                title: {
                    display: true,
                    text: '🌾  Crop Yield Progress Over Time',
                    color: '#e2e8f0',
                    font: { size: 15, weight: '700', family: "'Segoe UI', system-ui, sans-serif" },
                    padding: { top: 4, bottom: 24 }
                },
                tooltip: {
                    enabled: true,
                    backgroundColor: 'rgba(15,23,42,0.95)',
                    titleColor: '#94a3b8',
                    titleFont: { size: 11, weight: '600' },
                    bodyColor: '#e2e8f0',
                    bodyFont: { size: 13 },
                    borderColor: 'rgba(100,116,139,0.3)',
                    borderWidth: 1,
                    cornerRadius: 10,
                    padding: 14,
                    displayColors: true,
                    boxPadding: 6,
                    callbacks: {
                        title: (items) => '📅  ' + items[0].label,
                        label: (item) => '  ' + item.dataset.label + ':  ' + (item.parsed.y ?? 0).toLocaleString() + ' kg'
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Yield (kg)',
                        color: '#94a3b8',
                        font: { size: 11, weight: '600' },
                        padding: { bottom: 8 }
                    },
                    grid: {
                        color: 'rgba(148,163,184,0.08)',
                        drawTicks: false
                    },
                    ticks: {
                        callback: (val) => val.toLocaleString(),
                        color: '#64748b',
                        font: { size: 11 },
                        padding: 8
                    },
                    border: { display: false }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Harvest Date',
                        color: '#94a3b8',
                        font: { size: 11, weight: '600' },
                        padding: { top: 8 }
                    },
                    grid: {
                        color: 'rgba(148,163,184,0.05)',
                        drawTicks: false
                    },
                    ticks: {
                        color: '#64748b',
                        font: { size: 11 },
                        padding: 8,
                        maxRotation: 35,
                        autoSkip: true,
                        maxTicksLimit: 12
                    },
                    border: { display: false }
                }
            },
            animation: {
                duration: 1200,
                easing: 'easeOutQuart'
            },
            elements: {
                line: {
                    borderCapStyle: 'round',
                    borderJoinStyle: 'round'
                }
            }
        }
    });
}
