using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.CustomOrders;

/// <summary>
/// Especificación técnica de impresión para un pedido personalizado.
/// Entidad hija del aggregate CustomOrder.
/// </summary>
public class CustomSpecification : BaseEntity
{
    public Guid CustomOrderId { get; internal set; }
    public string PrintArea { get; internal set; } = default!;
    public string? Dimensions { get; internal set; }
    public string Technique { get; internal set; } = default!;
    public string? AdditionalNotes { get; internal set; }

    // Propiedades de navegación
    public CustomOrder CustomOrder { get; private set; } = default!;
}
