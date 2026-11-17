using BidPulse.Domain.Enums;
using BidPulse.Domain.Exceptions;
using BidPulse.Domain.ValueObjects;

namespace BidPulse.Domain.Entities;

/// <summary>
/// Representa una licitación pública u oportunidad B2B detectada, analizada por IA y gestionada por analistas.
/// </summary>
public sealed class Opportunity
{
    public Guid Id { get; private set; }
    public string ExternalCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Organization { get; private set; } = string.Empty;
    public decimal? EstimatedAmount { get; private set; }
    public string Currency { get; private set; } = "CLP";
    public string Source { get; private set; } = "MercadoPublico";
    public string? ExternalUrl { get; private set; }
    public DateTime PublishedAtUtc { get; private set; }
    public DateTime? ClosingAtUtc { get; private set; }
    public OpportunityStatus Status { get; private set; }
    public int? RelevanceScore { get; private set; }
    public string? ExecutiveSummary { get; private set; }
    public string CategoryTagsCsv { get; private set; } = string.Empty;
    public bool AlertDispatched { get; private set; }
    public DateTime? AlertDispatchedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? AnalyzedAtUtc { get; private set; }

    private Opportunity() { }

    public Opportunity(
        string externalCode,
        string title,
        string description,
        string organization,
        decimal? estimatedAmount,
        string currency,
        string source,
        string? externalUrl,
        DateTime publishedAtUtc,
        DateTime? closingAtUtc)
    {
        if (string.IsNullOrWhiteSpace(externalCode))
            throw new DomainException("ExternalCode is required for deduplication.");
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.");

        Id = Guid.NewGuid();
        ExternalCode = externalCode.Trim();
        Title = title.Trim();
        Description = description.Trim();
        Organization = organization.Trim();
        EstimatedAmount = estimatedAmount;
        Currency = string.IsNullOrWhiteSpace(currency) ? "CLP" : currency.Trim().ToUpperInvariant();
        Source = string.IsNullOrWhiteSpace(source) ? "MercadoPublico" : source.Trim();
        ExternalUrl = string.IsNullOrWhiteSpace(externalUrl) ? null : externalUrl.Trim();
        PublishedAtUtc = DateTime.SpecifyKind(publishedAtUtc, DateTimeKind.Utc);
        ClosingAtUtc = closingAtUtc.HasValue ? DateTime.SpecifyKind(closingAtUtc.Value, DateTimeKind.Utc) : null;
        Status = OpportunityStatus.New;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Aplica el análisis de IA validando que el puntaje esté en el rango 0–100.
    /// </summary>
    public void ApplyAiAnalysis(AiAnalysisResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.RelevanceScore is < 0 or > 100)
            throw new DomainException($"RelevanceScore must be between 0 and 100. Received: {result.RelevanceScore}.");

        if (string.IsNullOrWhiteSpace(result.ExecutiveSummary))
            throw new DomainException("ExecutiveSummary cannot be empty.");

        RelevanceScore = result.RelevanceScore;
        ExecutiveSummary = result.ExecutiveSummary.Trim();
        CategoryTagsCsv = string.Join(",", result.CategoryTags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase));
        AnalyzedAtUtc = DateTime.UtcNow;

        if (Status == OpportunityStatus.New)
        {
            Status = OpportunityStatus.Analyzed;
        }
    }

    /// <summary>
    /// Marca la oportunidad como alertada vía Webhook.
    /// </summary>
    public void MarkAlertSent()
    {
        AlertDispatched = true;
        AlertDispatchedAtUtc = DateTime.UtcNow;
        Status = OpportunityStatus.AlertSent;
    }

    /// <summary>
    /// Permite a un analista actualizar el estado operativo de la oportunidad.
    /// </summary>
    public void ChangeStatus(OpportunityStatus newStatus)
    {
        if (!Enum.IsDefined(newStatus))
            throw new DomainException($"Invalid status value: {newStatus}.");

        Status = newStatus;
    }

    /// <summary>
    /// Devuelve las etiquetas de rubro normalizadas como lista de solo lectura.
    /// </summary>
    public IReadOnlyList<string> GetCategoryTags() =>
        string.IsNullOrWhiteSpace(CategoryTagsCsv)
            ? Array.Empty<string>()
            : CategoryTagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
