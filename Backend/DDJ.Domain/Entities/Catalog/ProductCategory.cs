using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Catalog;

public class ProductCategory : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Id de la categoría padre. Null si es categoría raíz.
    /// Implementa jerarquía padre-hijo dentro de la misma tabla.
    /// </summary>
    public Guid? ParentCategoryId { get; private set; }

    // Propiedades de navegación
    public ProductCategory? ParentCategory { get; private set; }
    public ICollection<ProductCategory> SubCategories { get; private set; } = [];
    public ICollection<Product> Products { get; private set; } = [];
}