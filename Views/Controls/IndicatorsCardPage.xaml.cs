using KaaDebug.Core.Models.Plants;
using KaaDebug.Views.Controls;
using SkiaSharp;

namespace KaaDebug.Views.Controls;

// <summary>
/// Card reutilizável que representa um único indicador ambiental
/// (umidade do solo, umidade do ar, temperatura ou luminosidade) dentro
/// do carrossel horizontal da tela de Detalhes da Planta.
///
/// Centralizamos aqui a lógica de:
/// - Ícone/rótulo conforme o tipo de sensor
/// - Cor de status (dentro/fora da faixa ideal)
/// - Geração do mini-gráfico de tendência (Microcharts.Maui)
/// para evitar duplicar XAML/lógica 4 vezes na tela principal.
// </summary>       
    
public partial class IndicatorsCardPage : ContentView
{
    public IndicatorsCardPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Ponto único de entrada: popula todo o card a partir de um SensorIndicator.
    /// </summary>
    public void SetIndicator(SensorIndicator indicator)
    {
        var (icon, label) = GetIconAndLabel(indicator.Type);
        IconAndLabel.Text = $"{icon} {label}";

        ValueLabel.Text = FormatValue(indicator.CurrentValue, indicator.Unit);

        IdealRangeLabel.Text =
            $"Ideal: {indicator.IdealRange.Min:0.#}–{indicator.IdealRange.Max:0.#}{indicator.Unit}";

        ApplyStatus(indicator);
        BuildChart(indicator);
    }

    private static (string Icon, string Label) GetIconAndLabel(SensorType type) => type switch
    {
        SensorType.SoilMoisture => ("💧", "Umidade do solo"),
        SensorType.AirHumidity => ("🌫️", "Umidade do ar"),
        SensorType.Temperature => ("🌡️", "Temperatura"),
        SensorType.Luminosity => ("☀️", "Luminosidade"),
        _ => ("•", "Indicador")
    };

    private static string FormatValue(double value, string unit) =>
        $"{value:0.#}{unit}";

    private void ApplyStatus(SensorIndicator indicator)
    {
        if (indicator.IsWithinIdealRange)
        {
            StatusBadge.BackgroundColor = Color.FromArgb("#2E7D32");
            StatusLabel.Text = "Dentro do ideal";
            return;
        }

        var isBelow = indicator.CurrentValue < indicator.IdealRange.Min;
        StatusBadge.BackgroundColor = Color.FromArgb("#F0A93B");
        StatusLabel.Text = isBelow ? "Abaixo do ideal" : "Acima do ideal";
    }

    private void BuildChart(SensorIndicator indicator)
    {
        if (indicator.RecentHistory.Count < 2)
        {
            TrendChart.Drawable = null;
            return;
        }

        var values = indicator.RecentHistory
            .Select(r => (float)r.Value)
            .ToList();

        TrendChart.Drawable = new LineChartDrawable(
            values,
            (float)indicator.IdealRange.Min,
            (float)indicator.IdealRange.Max);

        TrendChart.Invalidate();
    }
}