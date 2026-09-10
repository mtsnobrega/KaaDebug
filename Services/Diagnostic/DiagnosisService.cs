/*
 * Responsabilidade:
 * Implementar a requisição de diagnóstico, fazendo o upload de imagens para a API.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Constrói um MultipartFormDataContent para 
 * encapsular o array de bytes da foto e despacha o arquivo físico para a BFF API 
 * através do método PostMultipartAsync do ApiClient.
 */
using KaaDebug.Core.Interfaces.Diagnostic;
using KaaDebug.Core.Models.Diagnostic;
using KaaDebug.Infrastructure.http;

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