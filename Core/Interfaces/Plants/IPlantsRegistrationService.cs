/*
 * Responsabilidade:
 * Contrato para o registro (criação) de uma nova planta e associação inicial de dispositivo.
 * 
 * Papel na arquitetura:
 * Define o modelo de requisição (CreatePlantRequest) e o resultado contendo o novo PlantId.
 */
namespace KaaDebug.Core.Interfaces.Plants
{
    public class CreatePlantRequest
    {
        public string Name { get; init; } = string.Empty;
        public string SpeciesId { get; init; } = string.Empty;

        /// <summary>
        /// Código do dispositivo ESP32, se o usuário optou por associar
        /// um dispositivo já durante o cadastro. Nulo se for pular essa etapa.
        /// </summary>
        public string? DeviceCode { get; init; }
    }

    public class CreatePlantResult
    {
        public bool Success { get; init; }
        public string? PlantId { get; init; }
        public string? ErrorMessage { get; init; }

        public static CreatePlantResult Ok(string plantId) => new() { Success = true, PlantId = plantId };
        public static CreatePlantResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantsRegistrationService
    {
        Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request);
    }
}
