using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>Id da planta recém-criada, usado para navegar aos Detalhes.</summary>
        public string? PlantId { get; init; }
        public string? ErrorMessage { get; init; }

        public static CreatePlantResult Ok(string plantId) => new() { Success = true, PlantId = plantId };
        public static CreatePlantResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para o cadastro de uma nova planta.
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   POST /plants
    /// que deve validar o código do dispositivo (se informado) antes de
    /// concluir o cadastro.
    /// </summary>
    public interface IPlantsRegistrationService
    {
        Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request);
    }
}
