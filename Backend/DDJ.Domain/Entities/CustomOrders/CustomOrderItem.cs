using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.CustomOrders;

/// <summary>
/// Ítem de prenda dentro de un pedido personalizado.
/// Entidad hija del aggregate CustomOrder.
/// </summary>
public class CustomOrderItem : BaseEntity
{
    public Guid CustomOrderId { get; internal set; }
    public string Description { get; internal set; } = default!;
    public int Quantity { get; internal set; }

    /// <summary>Null hasta que el staff define el precio en la cotización.</summary>
    public decimal? UnitPrice { get; internal set; }

    // Propiedades de navegación
    public CustomOrder CustomOrder { get; private set; } = default!;
}
