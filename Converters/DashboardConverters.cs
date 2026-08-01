using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Converters
{
    /// <summary>
    /// Converte PlantHealthStatus para a cor de fundo do badge exibido no card.
    /// Verde = saudável, Amarelo/laranja = atenção, Vermelho = crítico
    /// (apenas crítico usa vermelho, conforme diretriz de paleta do app:
    /// vermelho reservado para alertas críticos).
    /// </summary>
    public class HealthStatusToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                PlantHealthStatus.Healthy => Color.FromArgb("#2E7D32"),
                PlantHealthStatus.Attention => Color.FromArgb("#F0A93B"),
                PlantHealthStatus.Critical => Color.FromArgb("#C62828"),
                _ => Color.FromArgb("#8A9A8C")
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Converte PlantHealthStatus para o texto curto exibido no badge.
    /// </summary>
    public class HealthStatusToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                PlantHealthStatus.Healthy => "Saudável",
                PlantHealthStatus.Attention => "Atenção",
                PlantHealthStatus.Critical => "Crítico",
                _ => "—"
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Converte NotificationPriority para a cor do indicador (bolinha) na
    /// lista de notificações.
    /// </summary>
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                NotificationPriority.High => Color.FromArgb("#C62828"),
                NotificationPriority.Medium => Color.FromArgb("#F0A93B"),
                NotificationPriority.Low => Color.FromArgb("#6FA8DC"), // azul suave, conforme paleta para informações
                _ => Color.FromArgb("#8A9A8C")
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Usado para esconder o Label de StatusReason quando ele é nulo/vazio
    /// (plantas saudáveis não têm motivo de alerta).
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
