/*
 * Responsabilidade:
 * Contrato para consulta do catálogo mestre de espécies botânicas do sistema.
 * 
 * Papel na arquitetura:
 * Interface de leitura de dados globais.
 */

using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class SpeciesCatalogListResult
    {
        public bool Success { get; init; }
        public List<PlantSpecies>? Species { get; init; }
        public string? ErrorMessage { get; init; }

        public static SpeciesCatalogListResult Ok(List<PlantSpecies> species) => new() { Success = true, Species = species };
        public static SpeciesCatalogListResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantsCatalogService
    {
        Task<SpeciesCatalogListResult> GetAllSpeciesAsync();
    }
}
