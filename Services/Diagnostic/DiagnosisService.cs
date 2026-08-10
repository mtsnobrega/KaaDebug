using KaaDebug.Core.Interfaces.Diagnostic;
using KaaDebug.Core.Models.Diagnostic;
using KaaDebug.Infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Diagnostic;
public class DiagnosisService : IDiagnosisService
{
    private readonly ApiClient _apiClient;

    public DiagnosisService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IADiagnosisResult> AnalyzeImageAsync(string plantId, byte[] imageBytes)
    {
        using var content = new MultipartFormDataContent();
        using var imageContent = new ByteArrayContent(imageBytes);

        imageContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        content.Add(imageContent, "image", $"plant_{plantId}.jpg");

        var result = await _apiClient.PostMultipartAsync<DiagnosisResultDto>(
            ApiConstants.Plants.Diagnose(Guid.Parse(plantId)), content);

        if (!result.Success)
            return IADiagnosisResult.Fail(result.ErrorMessage!);

        return IADiagnosisResult.Ok(MapDiagnosis(result.Data!));
    }

    public async Task<List<DiagnosisResult>> GetDiagnosisHistoryAsync(string plantId)
    {
        var result = await _apiClient.GetAsync<List<DiagnosisResultDto>>(
            ApiConstants.Plants.GetDiagnoses(Guid.Parse(plantId)));

        if (!result.Success)
            return new List<DiagnosisResult>();

        return result.Data!.Select(MapDiagnosis).ToList();
    }

    private static DiagnosisResult MapDiagnosis(DiagnosisResultDto dto) => new()
    {
        Id = dto.Id.ToString(),
        PerformedAt = dto.PerformedAt,
        //IsHealthy = dto.IsHealthy,
        OverallObservation = dto.OverallObservation,
        Issues = dto.Issues.Select(i => new DiagnosisIssue
        {
            Name = i.Name,
            ConfidencePercent = i.ConfidencePercent,
            Description = i.Description,
            Recommendations = i.Recommendations
        }).ToList()
    };
}

/*
public class DiagnosisService : IDiagnosisService
{
    public async Task<IADiagnosisResult> AnalyzeImageAsync(string plantId, byte[] imageBytes)
    {
        // Simula o tempo de processamento da IA
        await Task.Delay(3000);

        var diagnosis = new DiagnosisResult
        {
            Id = Guid.NewGuid().ToString(),
            PerformedAt = DateTime.Now,
            OverallObservation = "A planta apresenta sinais visuais de estresse hídrico. As folhas mostram leve descoloração nas bordas.",
            Issues = new List<DiagnosisIssue>
        {
            new()
            {
                Name = "Estresse hídrico",
                ConfidencePercent = 87,
                Description = "A planta apresenta sinais de desidratação. As bordas das folhas estão levemente amareladas.",
                Recommendations = new List<string>
                {
                    "Verifique a umidade do solo antes de regar",
                    "Regue somente quando o solo estiver seco ao toque",
                    "Evite deixar água parada no pratinho"
                }
            },
            new()
            {
                Name = "Deficiência de nitrogênio",
                ConfidencePercent = 42,
                Description = "Possível deficiência nutricional, indicada pelo amarelecimento das folhas mais antigas.",
                Recommendations = new List<string>
                {
                    "Considere aplicar fertilizante equilibrado",
                    "Faça a adubação mensalmente durante o período de crescimento"
                }
            }
        }
        };

        return IADiagnosisResult.Ok(diagnosis);
    }

    public async Task<List<DiagnosisResult>> GetDiagnosisHistoryAsync(string plantId)
    {
        await Task.Delay(600);

        return new List<DiagnosisResult>
    {
        new()
        {
            Id = "d_old_1",
            PerformedAt = DateTime.Now.AddDays(-7),
            OverallObservation = "Planta com boa aparência geral.",
            Issues = new List<DiagnosisIssue>()
        },
        new()
        {
            Id = "d_old_2",
            PerformedAt = DateTime.Now.AddDays(-21),
            OverallObservation = "Sinais leves de excesso de sol direto.",
            Issues = new List<DiagnosisIssue>
            {
                new()
                {
                    Name = "Queimadura solar",
                    ConfidencePercent = 73,
                    Description = "Manchas esbranquiçadas nas folhas expostas ao sol.",
                    Recommendations = new List<string> { "Mova a planta para local com luz indireta" }
                }
            }
        }
    };
    }
}
*/