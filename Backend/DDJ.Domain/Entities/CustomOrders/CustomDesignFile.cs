using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.CustomOrders;

/// <summary>
/// Archivo de diseño subido a Cloudinary para un pedido personalizado.
/// Entidad hija del aggregate CustomOrder.
/// </summary>
public class CustomDesignFile : BaseEntity
{
    public Guid CustomOrderId { get; internal set; }
    public string FileUrl { get; internal set; } = default!;
    public string FileName { get; internal set; } = default!;
    public string FileType { get; internal set; } = default!;
    public DateTime UploadedAt { get; internal set; }

    // Propiedades de navegación
    public CustomOrder CustomOrder { get; private set; } = default!;
}
