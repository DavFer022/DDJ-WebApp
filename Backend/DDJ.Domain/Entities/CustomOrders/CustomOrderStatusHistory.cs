using DDJ.Domain.Common;
using DDJ.Domain.Entities.Identity;
using DDJ.Domain.Enums;

namespace DDJ.Domain.Entities.CustomOrders;

/// <summary>
/// Entrada inmutable del historial de cambios de estado en un pedido personalizado.
/// Se crea automáticamente por CustomOrder.TransitionTo() — nunca externamente.
/// </summary>
public class CustomOrderStatusHistory : BaseEntity
{
    public Guid CustomOrderId { get; internal set; }
    public CustomOrderStatus Status { get; internal set; }
    public string? Notes { get; internal set; }
    public Guid ChangedBy { get; internal set; }
    public DateTime ChangedAt { get; internal set; }

    // Propiedades de navegación
    public CustomOrder CustomOrder { get; private set; } = default!;
    public User ChangedByUser { get; private set; } = default!;
}
