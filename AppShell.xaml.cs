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

            
            Routing.RegisterRoute("RegisterPlant", typeof(RegisterPlantPage));
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
