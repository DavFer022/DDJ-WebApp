using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Catalog;

public class Price : BaseEntity
{
    public Guid ProductVariantId { get; private set; }
    public decimal BasePrice { get; private set; }
    public decimal? DiscountPrice { get; private set; }
    public string Currency { get; private set; } = "USD";

    /// <summary>Inicio de vigencia del precio. Null = sin límite inferior.</summary>
    public DateTime? ValidFrom { get; private set; }

    /// <summary>Fin de vigencia del precio. Null = sin límite superior (precio activo indefinidamente).</summary>
    public DateTime? ValidTo { get; private set; }

    /// <summary>Precio en vigencia si las fechas cubren el momento actual.</summary>
    public bool IsCurrentlyActive =>
        (ValidFrom == null || ValidFrom <= DateTime.UtcNow) &&
        (ValidTo == null || ValidTo >= DateTime.UtcNow);

    // Propiedades de navegación
    public ProductVariant ProductVariant { get; private set; } = default!;
}