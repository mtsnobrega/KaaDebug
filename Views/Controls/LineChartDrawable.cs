using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Desenha um gráfico de linha simples usando IDrawable nativo do .NET MAUI.
/// Substitui o Microcharts.Maui, eliminando a dependência do SkiaSharp
/// que causava crash no Windows com .NET MAUI 9 / Microsoft.Maui.Controls 9.0.120+.
///
/// Uso no XAML:
///   &lt;GraphicsView x:Name="ChartView" HeightRequest="80" /&gt;
///
/// Uso no code-behind:
///   var drawable = new LineChartDrawable(values, idealMin, idealMax);
///   ChartView.Drawable = drawable;
///   ChartView.Invalidate();
/// </summary>
public class LineChartDrawable : IDrawable
{
    private readonly List<float> _values;
    private readonly float _idealMin;
    private readonly float _idealMax;

    // Cor dos pontos dentro da faixa ideal: verde PlantCare
    private static readonly Color ColorOk = Color.FromArgb("#2E7D32");

    // Cor dos pontos fora da faixa ideal: laranja de atenção
    private static readonly Color ColorOut = Color.FromArgb("#F0A93B");

    // Cor da linha de conexão entre pontos
    private static readonly Color ColorLine = Color.FromArgb("#A5D6A7");

    // Cor da faixa de referência ideal (banda sombreada)
    private static readonly Color ColorBand = Color.FromArgb("#1A2E7D32"); // verde com 10% de opacidade

    public LineChartDrawable(IEnumerable<float> values, float idealMin, float idealMax)
    {
        _values = values.ToList();
        _idealMin = idealMin;
        _idealMax = idealMax;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_values.Count < 2) return;

        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        var padding = new Thickness(4, 6, 4, 6);

        var drawWidth = width - (float)(padding.Left + padding.Right);
        var drawHeight = height - (float)(padding.Top + padding.Bottom);

        // Calcula range de valores para escalar o gráfico
        var minVal = Math.Min(_values.Min(), _idealMin) - 1f;
        var maxVal = Math.Max(_values.Max(), _idealMax) + 1f;
        var range = maxVal - minVal;
        if (range <= 0) range = 1;

        // Função de mapeamento: valor → coordenada Y (invertido: maior valor = Y menor)
        float ToY(float value) =>
            (float)padding.Top + drawHeight - ((value - minVal) / range * drawHeight);

        // Função de mapeamento: índice → coordenada X
        float ToX(int index) =>
            (float)padding.Left + index / (float)(_values.Count - 1) * drawWidth;

        // Desenha a faixa ideal (banda verde suave)
        var bandTop = ToY(_idealMax);
        var bandBottom = ToY(_idealMin);
        canvas.FillColor = ColorBand;
        canvas.FillRectangle(
            (float)padding.Left, bandTop,
            drawWidth, bandBottom - bandTop);

        // Desenha as linhas entre pontos
        canvas.StrokeColor = ColorLine;
        canvas.StrokeSize = 1.5f;

        for (int i = 0; i < _values.Count - 1; i++)
        {
            canvas.DrawLine(
                ToX(i), ToY(_values[i]),
                ToX(i + 1), ToY(_values[i + 1]));
        }

        // Desenha os pontos coloridos conforme status ideal/fora
        foreach (var (value, index) in _values.Select((v, i) => (v, i)))
        {
            var isWithinIdeal = value >= _idealMin && value <= _idealMax;
            canvas.FillColor = isWithinIdeal ? ColorOk : ColorOut;
            canvas.FillCircle(ToX(index), ToY(value), 3f);
        }
    }
}
