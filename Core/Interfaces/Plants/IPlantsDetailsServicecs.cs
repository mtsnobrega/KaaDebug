using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class PlantDetailsResult
    {
        public bool Success { get; init; }
        public PlantDetails? Details { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantDetailsResult Ok(PlantDetails details) => new() { Success = true, Details = details };
        public static PlantDetailsResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para obtenção dos dados completos de uma planta.
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   GET /plants/{id}
    /// que deve agregar: dados cadastrais, leituras atuais dos sensores +
    /// histórico recente (24h), status do dispositivo, e notificações
    /// relacionadas a essa planta especificamente.
    /// </summary>
    public interface IPlantDetailsService
    {
        Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId);
    }
}
