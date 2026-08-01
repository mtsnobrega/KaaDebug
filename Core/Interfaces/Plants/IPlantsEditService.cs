using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class EditPlantRequest
    {
        public string PlantId { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Código do dispositivo ESP32. Nulo = manter o dispositivo atual sem alteração.
        /// String vazia = desassociar o dispositivo atual.
        /// String preenchida = associar novo dispositivo (ou substituir o atual).
        /// </summary>
        public string? DeviceCode { get; init; }
    }

    public class EditPlantResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static EditPlantResult Ok() => new() { Success = true };
        public static EditPlantResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para edição e exclusão de uma planta existente.
    /// A implementação real dependerá de endpoints futuros:
    ///   PUT  /plants/{id}
    ///   DELETE /plants/{id}
    /// </summary>
    public interface IPlantsEditService
    {
        Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request);
        Task<EditPlantResult> DeletePlantAsync(string plantId);
    }
}
