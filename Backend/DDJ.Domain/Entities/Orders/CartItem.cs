using DDJ.Domain.Common;
using DDJ.Domain.Entities.Catalog;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Orders;

/// <summary>
/// Ítem dentro de un carrito de compras. Entidad hija del aggregate Cart.
/// Solo puede ser creada o modificada a través del aggregate root Cart.
/// </summary>
public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime AddedAt { get; private set; }

    // Propiedades de navegación
    public Cart Cart { get; private set; } = default!;
    public ProductVariant ProductVariant { get; private set; } = default!;

    // Constructor privado – EF Core
    private CartItem() { }

    internal static CartItem Create(Guid cartId, Guid productVariantId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.", "CART_INVALID_QUANTITY");

        return new CartItem
        {
            CartId = cartId,
            ProductVariantId = productVariantId,
            Quantity = quantity,
            AddedAt = DateTime.UtcNow
        };
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.", "CART_INVALID_QUANTITY");

        Quantity = newQuantity;
    }
}