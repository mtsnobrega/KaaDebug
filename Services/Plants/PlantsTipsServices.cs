using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants
{
    /// <summary>
    /// Implementação temporária de IBudflowService.
    /// Retorna dicas fixas de Suculenta, independente do plantId,
    /// para validar o layout da tela.
    /// SUBSTITUIR pela implementação real quando o endpoint existir.
    /// </summary>
    public class PlantTipsService : IPlantTipsService
    {
        public async Task<PlantCareInfoResult> GetCareInfoAsync(string plantId)
        {
            await Task.Delay(700);

            var info = new PlantCareInfo
            {
                SpeciesName = "Suculenta",
                Summary = "Suculentas são plantas resistentes, perfeitas para iniciantes. Armazenam água em seus tecidos, o que as torna tolerantes a períodos de seca.",
                Curiosity = "O nome \"suculenta\" vem do latim sucus, que significa suco ou seiva — referência à sua capacidade de armazenar líquido.",
                Tips = new List<CareTip>
            {
                new() { Icon = "💧", Title = "Rega",
                    Description = "Regue apenas quando o solo estiver completamente seco. Em média, a cada 10-14 dias no verão e a cada 3-4 semanas no inverno. Evite encharcar." },

                new() { Icon = "☀️", Title = "Luminosidade",
                    Description = "Prefere luz solar direta por pelo menos 4-6 horas por dia. Perto de janelas com boa exposição ao sol é o ambiente ideal." },

                new() { Icon = "🌡️", Title = "Temperatura",
                    Description = "Prospera entre 18°C e 30°C. Tolera calor, mas é sensível a geadas. Evite temperaturas abaixo de 5°C." },

                new() { Icon = "🪴", Title = "Solo e vaso",
                    Description = "Use substrato específico para suculentas ou misture terra comum com areia grossa. O vaso deve ter furos de drenagem obrigatoriamente." },

                new() { Icon = "✂️", Title = "Poda",
                    Description = "Retire folhas secas ou danificadas na base com uma tesoura limpa. A poda é principalmente estética e não prejudica a planta." },

                new() { Icon = "🌿", Title = "Adubação",
                    Description = "Adubar uma vez por mês na primavera e verão com fertilizante específico para suculentas, diluído à metade da dose recomendada." },

                new() { Icon = "⚠️", Title = "Problemas comuns",
                    Description = "Folhas amolecidas ou translúcidas indicam excesso de água. Folhas enrugadas indicam falta de água ou rega insuficiente. Manchas escuras podem indicar fungos." }
            }
            };

            return PlantCareInfoResult.Ok(info);
        }
    }

    // Esqueleto real:
    // public class BudflowService : IBudflowService
    // {
    //     private readonly HttpClient _httpClient;
    //     public BudflowService(HttpClient httpClient) => _httpClient = httpClient;
    //     public async Task<BudflowInfoResult> GetCareInfoAsync(string plantId)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync($"plants/{plantId}/care-tips");
    //             if (!response.IsSuccessStatusCode)
    //                 return BudflowInfoResult.Fail("Não foi possível carregar as dicas.");
    //             var info = await response.Content.ReadFromJsonAsync<BudflowInfo>();
    //             return BudflowInfoResult.Ok(info!);
    //         }
    //         catch (HttpRequestException) { return BudflowInfoResult.Fail("Sem conexão."); }
    //     }
    // }
}
