namespace BidPulse.Domain.Exceptions;

/// <summary>
/// Excepción para violaciones de reglas o invariantes de dominio.
/// </summary>
public class DomainException(string message) : Exception(message);
