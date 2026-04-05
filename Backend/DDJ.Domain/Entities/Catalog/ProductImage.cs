using DDJ.Domain.Common;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Catalog;

/// <summary>
/// Entidad hija del aggregate Product.
/// Representa una imagen asociada al producto o a una variante específica.
/// </summary>
public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }

    /// <summary>Null si la imagen es del producto general. No-null si es específica a una variante.</summary>
    public Guid? VariantId { get; private set; }

    public string ImageUrl { get; private set; } = default!;
    public bool IsPrimary { get; private set; }
    public int DisplayOrder { get; private set; }

    // Propiedades de navegación
    public Product Product { get; private set; } = default!;
    public ProductVariant? Variant { get; private set; }

    // Constructor privado – EF Core
    private ProductImage() { }

    internal static ProductImage Create(Guid productId, string imageUrl, bool isPrimary, int displayOrder, Guid? variantId = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("La URL de la imagen es requerida.", "IMAGE_MISSING_URL");

        return new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            IsPrimary = isPrimary,
            DisplayOrder = displayOrder,
            VariantId = variantId
        };
    }

    /// <summary>Quita el flag de imagen primaria (llamado por Product al designar una nueva primaria).</summary>
    internal void UnsetPrimary() => IsPrimary = false;
}