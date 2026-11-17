using BidPulse.Domain.Exceptions;

namespace BidPulse.Domain.Entities;

/// <summary>
/// Regla configurable de monitoreo: palabras clave de negocio, umbral de alerta y destino Webhook.
/// </summary>
public sealed class MonitoringRule
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string KeywordsCsv { get; private set; } = string.Empty;
    public int AlertThresholdScore { get; private set; } = 75;
    public string? WebhookUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime UpdatedAtUtc { get; private set; }

    private MonitoringRule() { }

    public MonitoringRule(string name, string keywordsCsv, int alertThresholdScore = 75, string? webhookUrl = null)
    {
        Update(name, keywordsCsv, alertThresholdScore, webhookUrl, true);
        Id = Guid.NewGuid();
    }

    public void Update(string name, string keywordsCsv, int alertThresholdScore, string? webhookUrl, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Rule name is required.");
        if (alertThresholdScore is < 0 or > 100)
            throw new DomainException("Alert threshold score must be between 0 and 100.");

        Name = name.Trim();
        KeywordsCsv = string.Join(",", keywordsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(k => k.ToLowerInvariant())
            .Distinct());
        AlertThresholdScore = alertThresholdScore;
        WebhookUrl = string.IsNullOrWhiteSpace(webhookUrl) ? null : webhookUrl.Trim();
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public IReadOnlyList<string> GetKeywords() =>
        string.IsNullOrWhiteSpace(KeywordsCsv)
            ? Array.Empty<string>()
            : KeywordsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
