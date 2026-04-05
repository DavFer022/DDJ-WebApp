using DDJ.Domain.Common;
using DDJ.Domain.Entities.Inventory;
using DDJ.Domain.Entities.Orders;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Catalog;

/// <summary>
/// Entidad hija del aggregate Product.
/// Representa una combinación única talla+color con su propio SKU e inventario.
/// </summary>
public class ProductVariant : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Color { get; private set; } = default!;
    public string Size { get; private set; } = default!;
    public bool IsActive { get; private set; }

    // Propiedades de navegación
    public Product Product { get; private set; } = default!;
    public ICollection<ProductImage> ProductImages { get; private set; } = [];
    public ICollection<Price> Prices { get; private set; } = [];

    /// <summary>Relación 1-to-1: cada variante tiene exactamente un registro de inventario.</summary>
    public InventoryItem? InventoryItem { get; private set; }

    public ICollection<CartItem> CartItems { get; private set; } = [];
    public ICollection<OrderItem> OrderItems { get; private set; } = [];

    // Constructor privado – EF Core
    private ProductVariant() { }

    internal static ProductVariant Create(Guid productId, string sku, string size, string color, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("El SKU es requerido.", "VARIANT_MISSING_SKU");
        if (string.IsNullOrWhiteSpace(size))
            throw new DomainException("La talla es requerida.", "VARIANT_MISSING_SIZE");
        if (string.IsNullOrWhiteSpace(color))
            throw new DomainException("El color es requerido.", "VARIANT_MISSING_COLOR");

        return new ProductVariant
        {
            ProductId = productId,
            Sku = sku,
            Size = size,
            Color = color,
            Description = description,
            IsActive = true
        };
    }

    internal void Deactivate()
    {
        IsActive = false;
    }
}
