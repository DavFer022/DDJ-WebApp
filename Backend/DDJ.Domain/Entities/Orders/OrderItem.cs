using DDJ.Domain.Common;
using DDJ.Domain.Entities.Catalog;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Orders;

/// <summary>
/// Ítem de un pedido confirmado. Entidad hija del aggregate Order.
/// Solo puede ser creada a través de Order.AddItem().
/// Subtotal es calculado — nunca se persiste como campo independiente.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    /// <summary>Calculado. No mapear como columna independiente en EF.</summary>
    public decimal Subtotal => Quantity * UnitPrice;

    // Propiedades de navegación
    public Order Order { get; private set; } = default!;
    public ProductVariant ProductVariant { get; private set; } = default!;

    // Constructor privado – EF Core
    private OrderItem() { }

    internal static OrderItem Create(Guid orderId, Guid productVariantId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.", "ORDER_INVALID_QUANTITY");
        if (unitPrice <= 0)
            throw new DomainException("El precio unitario debe ser mayor a cero.", "ORDER_INVALID_PRICE");

        return new OrderItem
        {
            OrderId = orderId,
            ProductVariantId = productVariantId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    internal void IncreaseQuantity(int additional)
    {
        if (additional <= 0)
            throw new DomainException("El adicional debe ser mayor a cero.", "ORDER_INVALID_QUANTITY");

        Quantity += additional;
    }
}
