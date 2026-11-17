namespace BidPulse.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una entidad solicitada no existe.
/// </summary>
public sealed class NotFoundException(string message) : Exception(message);
