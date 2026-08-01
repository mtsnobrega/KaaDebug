using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class PlantCareInfoResult
    {
        public bool Success { get; init; }
        public PlantCareInfo? CareInfo { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantCareInfoResult Ok(PlantCareInfo info) => new() { Success = true, CareInfo = info };
        public static PlantCareInfoResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para obtenção das dicas de cuidados de uma planta específica.
    /// A implementação real dependerá de um endpoint futuro:
    ///   GET /plants/{id}/care-tips
    /// que retorna as informações baseadas na espécie da planta.
    /// </summary>
    public interface IPlantTipsService
    {
        Task<PlantCareInfoResult> GetCareInfoAsync(string plantId);
    }
}
