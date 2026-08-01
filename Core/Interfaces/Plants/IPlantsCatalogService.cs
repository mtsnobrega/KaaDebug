using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// Abstração para o catálogo de espécies cadastradas no sistema.
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   GET /species
    /// Esse catálogo é provavelmente mantido pela equipe/admin do sistema,
    /// e não pelo usuário final.
    /// </summary>
    public interface IPlantsCatalogService
    {
        Task<SpeciesCatalogListResult> GetAllSpeciesAsync();
    }
}
