using DDJ.Domain.Common;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Catalog;

/// <summary>
/// Aggregate root del catálogo de productos.
/// Gestiona variantes, imágenes y activación/desactivación del producto.
/// </summary>
public class Product : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];

    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    // Propiedades de navegación
    public ProductCategory Category { get; private set; } = default!;

    // Constructor privado – EF Core
    private Product() { }

    // ─────────────────────────────────────────────
    // Factory method
    // ─────────────────────────────────────────────

    /// <summary>Crea un producto nuevo en estado activo.</summary>
    public static Product Create(string name, string slug, Guid categoryId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del producto es requerido.", "PRODUCT_MISSING_NAME");
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("El slug del producto es requerido.", "PRODUCT_MISSING_SLUG");
        if (categoryId == Guid.Empty)
            throw new DomainException("La categoría es requerida.", "PRODUCT_MISSING_CATEGORY");

        return new Product
        {
            Name = name,
            Slug = slug,
            CategoryId = categoryId,
            Description = description,
            IsActive = true
        };
    }

    // ─────────────────────────────────────────────
    // Comportamientos del dominio
    // ─────────────────────────────────────────────

    /// <summary>Actualiza los datos editables del producto.</summary>
    public void Update(string name, string slug, Guid categoryId, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del producto es requerido.", "PRODUCT_MISSING_NAME");
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("El slug del producto es requerido.", "PRODUCT_MISSING_SLUG");

        Name = name;
        Slug = slug;
        CategoryId = categoryId;
        Description = description;
    }

    /// <summary>Activa el producto para que sea visible en el catálogo.</summary>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException("El producto ya está activo.", "PRODUCT_ALREADY_ACTIVE");
        if (!_variants.Any(v => v.IsActive))
            throw new DomainException(
                "No se puede activar un producto sin variantes activas.",
                "PRODUCT_NO_ACTIVE_VARIANTS");

        IsActive = true;
    }

    /// <summary>Desactiva el producto y lo oculta del catálogo público.</summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("El producto ya está inactivo.", "PRODUCT_ALREADY_INACTIVE");

        IsActive = false;

        // Desactivar también todas las variantes
        foreach (var variant in _variants.Where(v => v.IsActive))
            variant.Deactivate();
    }

    /// <summary>Agrega una variante al producto.</summary>
    public void AddVariant(string sku, string size, string color, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("El SKU es requerido.", "VARIANT_MISSING_SKU");
        if (_variants.Any(v => v.Sku == sku))
            throw new DomainException($"Ya existe una variante con el SKU '{sku}'.", "VARIANT_DUPLICATE_SKU");

        _variants.Add(ProductVariant.Create(Id, sku, size, color, description));
    }

    /// <summary>Agrega una imagen al producto.</summary>
    public void AddImage(string imageUrl, bool isPrimary = false, Guid? variantId = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("La URL de la imagen es requerida.", "IMAGE_MISSING_URL");

        // Solo puede haber una imagen primaria
        if (isPrimary)
        {
            foreach (var img in _images.Where(i => i.IsPrimary))
                img.UnsetPrimary();
        }

        int displayOrder = _images.Count;
        _images.Add(ProductImage.Create(Id, imageUrl, isPrimary, displayOrder, variantId));
    }
}
