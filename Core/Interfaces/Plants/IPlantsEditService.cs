/*
 * Responsabilidade:
 * Contrato para operações de mutação (Atualizar e Excluir) de uma planta existente.
 * 
 * Papel na arquitetura:
 * Isola a complexidade dos métodos HTTP PUT e DELETE, fornecendo métodos 
 * semânticos (UpdatePlantAsync, DeletePlantAsync) para a View.
 */

namespace KaaDebug.Core.Interfaces.Plants
{
    public class EditPlantRequest
    {
        public string PlantId { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? DeviceCode { get; init; }
    }

    public class EditPlantResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static EditPlantResult Ok() => new() { Success = true };
        public static EditPlantResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantsEditService
    {
        Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request);
        Task<EditPlantResult> DeletePlantAsync(string plantId);
    }
}
