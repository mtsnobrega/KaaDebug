using KaaDebug.Core.Models.Diagnostic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Diagnostic
{
    public class IADiagnosisResult
    {
        public bool Success { get; init; }
        public DiagnosisResult? Diagnosis { get; init; }
        public string? ErrorMessage { get; init; }

        public static IADiagnosisResult Ok(DiagnosisResult diagnosis) =>
            new() { Success = true, Diagnosis = diagnosis };
        public static IADiagnosisResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para o envio de imagem à IA e obtenção do diagnóstico.
    /// A implementação real dependerá de um endpoint futuro:
    ///   POST /plants/{id}/diagnosis
    /// com a imagem em base64 no corpo da requisição.
    /// </summary>
    public interface IDiagnosisService
    {
        Task<IADiagnosisResult> AnalyzeImageAsync(string plantId, byte[] imageBytes);
        Task<List<DiagnosisResult>> GetDiagnosisHistoryAsync(string plantId);
    }
}
