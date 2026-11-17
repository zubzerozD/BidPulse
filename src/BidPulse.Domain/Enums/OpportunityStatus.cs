namespace BidPulse.Domain.Enums;

/// <summary>
/// Ciclo de vida de una oportunidad o licitación B2B.
/// </summary>
public enum OpportunityStatus
{
    New = 0,
    Analyzed = 1,
    AlertSent = 2,
    Discarded = 3
}
