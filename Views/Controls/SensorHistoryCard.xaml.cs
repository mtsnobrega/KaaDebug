using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;
using SkiaSharp;
using System.IO;

namespace KaaDebug.Views.Controls;

public partial class SensorHistoryCard : ContentView
{
    public SensorHistoryCard()
    {
        InitializeComponent();
    }

    public void SetData(SensorHistory history, HistoryPeriod period)
    {
        var (icon, name) = GetIconAndName(history.Type);
        SensorIconLabel.Text = icon;
        SensorNameLabel.Text = name;
        IdealRangeLabel.Text =
            $"Ideal: {history.IdealRange.Min:0.#}–{history.IdealRange.Max:0.#}{history.Unit}";

        MinValueLabel.Text = history.MinValue.HasValue
            ? $"{history.MinValue:0.#}{history.Unit}" : "—";
        AvgValueLabel.Text = history.AvgValue.HasValue
            ? $"{history.AvgValue:0.#}{history.Unit}" : "—";
        MaxValueLabel.Text = history.MaxValue.HasValue
            ? $"{history.MaxValue:0.#}{history.Unit}" : "—";

        if (history.Readings.Count > 0)
            XAxisStartLabel.Text = FormatXAxisLabel(history.Readings[0].Timestamp, period);

        BuildChart(history);
    }

    private static (string Icon, string Name) GetIconAndName(SensorType type) => type switch
    {
        SensorType.SoilMoisture => ("💧", "Umidade do solo"),
        SensorType.AirHumidity => ("🌫️", "Umidade do ar"),
        SensorType.Temperature => ("🌡️", "Temperatura"),
        SensorType.Luminosity => ("☀️", "Luminosidade"),
        _ => ("•", "Sensor")
    };

    private static string FormatXAxisLabel(DateTime dt, HistoryPeriod period) => period switch
    {
        HistoryPeriod.Last24Hours => dt.ToString("HH:mm"),
        _ => dt.ToString("dd/MM")
    };

    private void BuildChart(SensorHistory history)
    {
        if (history.Readings.Count < 2)
        {
            HistoryChart.Drawable = null;
            return;
        }

        var values = history.Readings
            .Select(r => (float)r.Value)
            .ToList();

        HistoryChart.Drawable = new LineChartDrawable(
            values,
            (float)history.IdealRange.Min,
            (float)history.IdealRange.Max);

        HistoryChart.Invalidate();
    }
}