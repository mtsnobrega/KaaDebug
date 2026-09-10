/*
 * Responsabilidade:
 * Representar a estrutura de dados e as entidades de negócio utilizadas pelo aplicativo.
 * 
 * Papel na arquitetura:
 * Camada de Domínio (Core Models). Estes modelos são independentes de framework visual 
 * e de bibliotecas de rede. Eles atuam como a "Linguagem Ubíqua" do sistema, 
 * definindo Enums (como PlantHealthStatus), agregados de informações 
 * (como DashboardData) e Tipos de Valor (como IdealRange).
 */

namespace KaaDebug.Core.Models.Plants
{
    public class IdealRange
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
    public class SpeciesIdealParameters
    {
        public IdealRange SoilMoisture { get; set; } = new();   // % de umidade do solo
        public IdealRange AirHumidity { get; set; } = new();    // % de umidade do ar
        public IdealRange Temperature { get; set; } = new();    // °C
        public IdealRange Luminosity { get; set; } = new();     // lux
    }
    public class PlantSpecies
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public SpeciesIdealParameters IdealParameters { get; set; } = new();
    }
}
