using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Plants
{
    /// <summary>
    /// Faixa ideal de um parâmetro ambiental para uma espécie, usada como
    /// referência para geração de alertas (ex: solo fora da faixa = alerta).
    /// </summary>
    public class IdealRange
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    /// <summary>
    /// Conjunto completo de parâmetros ideais de monitoramento para uma espécie.
    /// Esses valores são carregados automaticamente ao selecionar a espécie
    /// durante o cadastro da planta, e servirão de referência para o backend
    /// gerar alertas (ex: Central de Notificações) quando os sensores do ESP32
    /// detectarem leituras fora dessas faixas.
    /// </summary>
    public class SpeciesIdealParameters
    {
        public IdealRange SoilMoisture { get; set; } = new();   // % de umidade do solo
        public IdealRange AirHumidity { get; set; } = new();    // % de umidade do ar
        public IdealRange Temperature { get; set; } = new();    // °C
        public IdealRange Luminosity { get; set; } = new();     // lux
    }

    /// <summary>
    /// Espécie de planta previamente cadastrada no catálogo do sistema,
    /// usada como referência para o cadastro de novas plantas.
    /// </summary>
    public class PlantSpecies
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public SpeciesIdealParameters IdealParameters { get; set; } = new();
    }
}
