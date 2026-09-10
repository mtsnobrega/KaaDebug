/*
 * Responsabilidade:
 * Converter resultados booleanos de diagnósticos em representações visuais (Ícones/Emojis).
 * 
 * Papel na arquitetura:
 * Camada de Apresentação. Isola a lógica de formatação visual do histórico de IA,
 * permitindo que o XAML resolva automaticamente o ícone com base no estado "IsHealthy".
 */
using System.Globalization;

namespace KaaDebug.Converters
{
    /// <summary>
    /// Converte o bool IsHealthy do DiagnosisResult para o ícone exibido
    /// na linha do histórico de diagnósticos.
    /// </summary>
    public class DiagnosisStatusIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            value is true ? "✅" : "⚠️";

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
