using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
