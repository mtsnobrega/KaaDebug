using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Diagnostic
{
    public class DiagnosisIssue
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>Nível de confiança da IA, de 0 a 100.</summary>
        public int ConfidencePercent { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new();
    }

    public class DiagnosisResult
    {
        public string Id { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }

        /// <summary>
        /// Lista de possíveis problemas identificados pela IA, ordenados
        /// por confiança decrescente. Pode ser vazia se a planta estiver saudável.
        /// </summary>
        public List<DiagnosisIssue> Issues { get; set; } = new();

        public bool IsHealthy => Issues.Count == 0;
        public string? OverallObservation { get; set; }
    }
}
