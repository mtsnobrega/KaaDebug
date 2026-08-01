using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Converters
{
    /// <summary>
    /// Cor de fundo do card: branco para lidas, verde suave para não lidas.
    /// Ajuda o usuário a identificar rapidamente o que ainda não viu.
    /// </summary>
    public class BoolToNotificationBgConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isRead = value is true;
            return isRead ? Colors.White : Color.FromArgb("#F0FAF0");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Borda do card: cinza suave para lidas, verde para não lidas.
    /// </summary>
    public class BoolToNotificationStrokeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isRead = value is true;
            return new SolidColorBrush(
                //isRead ? Color.FromArgb("#E3EFE4") : Color.FromArgb("#A5D6A7"));
                isRead? Color.FromArgb("#775b46") : Color.FromArgb("#775b46"));
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Inverte um bool — usado para mostrar a bolinha de "não lida"
    /// (IsVisible = !IsRead).
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            value is false;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            value is false;
    }

    /// <summary>
    /// Converte uma DateTime para texto relativo legível pelo usuário doméstico:
    /// "Agora", "Há 5 min", "Há 2h", "Ontem", "3 dias atrás", "dd/MM/yyyy".
    /// Mais humano e mais util do que a data absoluta nas notificações.
    /// </summary>
    public class DateToRelativeTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime dt) return string.Empty;

            var elapsed = DateTime.Now - dt;

            if (elapsed.TotalMinutes < 1) return "Agora";
            if (elapsed.TotalMinutes < 60) return $"Há {(int)elapsed.TotalMinutes} min";
            if (elapsed.TotalHours < 24) return $"Há {(int)elapsed.TotalHours}h";
            if (elapsed.TotalDays < 2) return "Ontem";
            if (elapsed.TotalDays < 7) return $"Há {(int)elapsed.TotalDays} dias";

            return dt.ToString("dd/MM/yyyy");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
