namespace BidPulse.Domain.ValueObjects;

/// <summary>
/// Resultado estructurado del análisis y scoring ejecutado por IA sobre una oportunidad.
/// </summary>
public sealed record AiAnalysisResult(
    string ExecutiveSummary,
    int RelevanceScore,
    IReadOnlyList<string> CategoryTags,
    string? Reasoning = null);
