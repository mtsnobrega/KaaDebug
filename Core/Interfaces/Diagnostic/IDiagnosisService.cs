/*
 * Responsabilidade:
 * Definir o contrato para as operações de Inteligência Artificial (Visão Computacional).
 * 
 * Papel na arquitetura:
 * Contrato de domínio (Core). Abstrai a complexidade do envio da imagem (byte[]) 
 * para análise e a recuperação do histórico de laudos fitossanitários gerados.
 */
using KaaDebug.Core.Models.Diagnostic;

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
    public interface IDiagnosisService
    {
        Task<IADiagnosisResult> AnalyzeImageAsync(string plantId, byte[] imageBytes);
        Task<List<DiagnosisResult>> GetDiagnosisHistoryAsync(string plantId);
    }
}
