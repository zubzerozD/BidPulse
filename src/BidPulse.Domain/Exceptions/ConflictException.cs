namespace BidPulse.Domain.Exceptions;

/// <summary>
/// Excepción lanzada ante conflictos de unicidad o estado (p. ej. email o código externo duplicado).
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
