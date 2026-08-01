using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Plants
{
    public class CareTip
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Informações completas de cuidados para uma espécie de planta.
    /// Exibidas na tela de Dicas de Cuidados, acessada a partir de Detalhes.
    /// </summary>
    public class PlantCareInfo
    {
        public string SpeciesName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? Curiosity { get; set; }
        public List<CareTip> Tips { get; set; } = new();
    }
}
