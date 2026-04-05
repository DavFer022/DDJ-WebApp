using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Audit;

/// <summary>
/// Registro inmutable de cambios en entidades del dominio.
/// Captura estado antes/después para trazabilidad completa.
/// </summary>
public class AuditLog : BaseEntity
{
    public string EntityName { get; private set; } = default!;
    public string EntityId { get; private set; } = default!;
    public string Action { get; private set; } = default!; // CREATE, UPDATE, DELETE
    public string? Changes { get; private set; }           // JSON con Before/After
    public Guid? UserId { get; private set; }              // Quién realizó el cambio (null si sistema)
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
