/*
 * Responsabilidade:
 * Definir a estrutura de navegação principal e o roteamento de páginas secundárias.
 * 
 * Papel na arquitetura:
 * Atua como o gerenciador de rotas e contêiner principal da interface de usuário. 
 * Isola a lógica de navegação profunda (Deep Linking) das demais páginas.
 * 
 * Estrutura visual:
 * Define abas (TabBar) para as funções principais e registra rotas nomeadas para 
 * navegação baseada em URI (ex: "SelectSpecies", "PlantTips").
 */

using KaaDebug.Views.Care;
using KaaDebug.Views.Devices;
using KaaDebug.Views.Diagnostic;
using KaaDebug.Views.Plants;

namespace KaaDebug
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            
            //Routing.RegisterRoute("RegisterPlant", typeof(RegisterPlantPage));
            Routing.RegisterRoute("SelectSpecies", typeof(SelectSpeciesPage));
            Routing.RegisterRoute("PlantsDetails", typeof(PlantsDetailsPage));
            Routing.RegisterRoute("EditPlant", typeof(PlantsEditPage));
            Routing.RegisterRoute("PlantTips", typeof(PlantTipsPage));
            Routing.RegisterRoute("RegisterDevice", typeof(RegisterDevicePage));
            Routing.RegisterRoute("AiDiagnosis", typeof(IADiagnosisPage));
            Routing.RegisterRoute("PlantHistory", typeof(PlantHistoryPage));
        }
    }
}
