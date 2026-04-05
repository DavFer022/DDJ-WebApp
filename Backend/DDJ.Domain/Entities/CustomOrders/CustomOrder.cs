using DDJ.Domain.Common;
using DDJ.Domain.Entities.Customers;
using DDJ.Domain.Entities.Identity;
using DDJ.Domain.Entities.Shipping;
using DDJ.Domain.Enums;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.CustomOrders;

/// <summary>
/// Aggregate root del pedido personalizado DTF/Sublimación.
/// Ciclo de vida: PendingReview → Quoted → Approved → InDesign → InPrinting → InFinishing → ReadyToShip → Shipped → Completed.
/// El admin/staff controla todas las transiciones de estado.
/// </summary>
public class CustomOrder : AuditableEntity
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public CustomOrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public decimal? QuotedAmount { get; private set; }
    public Guid? ShippingAddressId { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<CustomOrderItem> _items = [];
    private readonly List<CustomDesignFile> _designFiles = [];
    private readonly List<CustomSpecification> _specifications = [];
    private readonly List<CustomOrderStatusHistory> _statusHistory = [];

    public IReadOnlyCollection<CustomOrderItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<CustomDesignFile> DesignFiles => _designFiles.AsReadOnly();
    public IReadOnlyCollection<CustomSpecification> Specifications => _specifications.AsReadOnly();
    public IReadOnlyCollection<CustomOrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    // Propiedades de navegación
    public Customer Customer { get; private set; } = default!;
    public ShippingAddress? ShippingAddress { get; private set; }

    // Constructor privado – EF Core
    private CustomOrder() { }

    // ─────────────────────────────────────────────
    // Factory method
    // ─────────────────────────────────────────────

    /// <summary>
    /// Crea un pedido personalizado en estado PendingReview.
    /// El cliente crea la solicitud inicial; el staff la revisa y cotiza.
    /// </summary>
    public static CustomOrder Create(Guid customerId, string? notes = null)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El customerId es requerido.", "CUSTOM_ORDER_MISSING_CUSTOMER");

        return new CustomOrder
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            Status = CustomOrderStatus.PendingReview,
            PaymentStatus = PaymentStatus.Pending,
            Notes = notes
        };
    }

    // ─────────────────────────────────────────────
    // Gestión de ítems, archivos y especificaciones
    // ─────────────────────────────────────────────

    /// <summary>Agrega un ítem de prenda a la solicitud. Solo válido en PendingReview o UnderReview.</summary>
    public void AddItem(string description, int quantity)
    {
        EnsureEditableStatus();

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del ítem es requerida.", "CUSTOM_ORDER_ITEM_MISSING_DESC");
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.", "CUSTOM_ORDER_INVALID_QUANTITY");

        _items.Add(new CustomOrderItem
        {
            CustomOrderId = Id,
            Description = description,
            Quantity = quantity
        });
    }

    /// <summary>Agrega un archivo de diseño subido a Cloudinary.</summary>
    public void AddDesignFile(string fileUrl, string fileName, string fileType)
    {
        EnsureEditableStatus();

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new DomainException("La URL del archivo es requerida.", "CUSTOM_ORDER_MISSING_FILE_URL");

        _designFiles.Add(new CustomDesignFile
        {
            CustomOrderId = Id,
            FileUrl = fileUrl,
            FileName = fileName,
            FileType = fileType,
            UploadedAt = DateTime.UtcNow
        });
    }

    /// <summary>Define la especificación de impresión (área, dimensiones, técnica).</summary>
    public void SetSpecification(string printArea, string technique, string? dimensions = null, string? additionalNotes = null)
    {
        EnsureEditableStatus();

        if (string.IsNullOrWhiteSpace(printArea))
            throw new DomainException("El área de impresión es requerida.", "CUSTOM_ORDER_MISSING_PRINT_AREA");
        if (string.IsNullOrWhiteSpace(technique))
            throw new DomainException("La técnica de impresión es requerida.", "CUSTOM_ORDER_MISSING_TECHNIQUE");

        _specifications.Add(new CustomSpecification
        {
            CustomOrderId = Id,
            PrintArea = printArea,
            Technique = technique,
            Dimensions = dimensions,
            AdditionalNotes = additionalNotes
        });
    }

    /// <summary>Asigna la dirección de envío (puede hacerse antes o durante la cotización).</summary>
    public void AssignShippingAddress(Guid shippingAddressId)
    {
        if (shippingAddressId == Guid.Empty)
            throw new DomainException("La dirección de envío es requerida.", "CUSTOM_ORDER_MISSING_SHIPPING");

        ShippingAddressId = shippingAddressId;
    }

    // ─────────────────────────────────────────────
    // Transiciones de estado (staff/admin)
    // ─────────────────────────────────────────────

    /// <summary>Staff pone el pedido en revisión activa.</summary>
    public void StartReview(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.PendingReview, "iniciar revisión");
        TransitionTo(CustomOrderStatus.UnderReview, changedBy);
    }

    /// <summary>
    /// Staff envía una cotización al cliente.
    /// El monto debe ser mayor a cero.
    /// </summary>
    public void Quote(decimal amount, Guid changedBy, string? notes = null)
    {
        if (Status is not (CustomOrderStatus.UnderReview or CustomOrderStatus.PendingReview))
            throw new DomainException("Solo se puede cotizar un pedido en revisión.", "CUSTOM_ORDER_CANNOT_QUOTE");
        if (amount <= 0)
            throw new DomainException("El monto de cotización debe ser mayor a cero.", "CUSTOM_ORDER_INVALID_AMOUNT");
        if (!_items.Any())
            throw new DomainException("El pedido no tiene ítems para cotizar.", "CUSTOM_ORDER_NO_ITEMS");

        QuotedAmount = amount;
        TransitionTo(CustomOrderStatus.Quoted, changedBy, notes);
    }

    /// <summary>Cliente (o admin en su nombre) aprueba la cotización y confirma el pago.</summary>
    public void ApproveAndConfirmPayment(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.Quoted, "aprobar cotización");

        PaymentStatus = PaymentStatus.Confirmed;
        TransitionTo(CustomOrderStatus.Approved, changedBy);
    }

    /// <summary>Staff inicia el trabajo de diseño.</summary>
    public void StartDesign(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.Approved, "iniciar diseño");
        TransitionTo(CustomOrderStatus.InDesign, changedBy);
    }

    /// <summary>Staff inicia la impresión.</summary>
    public void StartPrinting(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.InDesign, "iniciar impresión");
        TransitionTo(CustomOrderStatus.InPrinting, changedBy);
    }

    /// <summary>Staff inicia el acabado final.</summary>
    public void StartFinishing(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.InPrinting, "iniciar acabado");
        TransitionTo(CustomOrderStatus.InFinishing, changedBy);
    }

    /// <summary>Pedido listo para despacho.</summary>
    public void MarkReadyToShip(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.InFinishing, "marcar listo para envío");
        TransitionTo(CustomOrderStatus.ReadyToShip, changedBy);
    }

    /// <summary>Pedido despachado al cliente.</summary>
    public void MarkAsShipped(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.ReadyToShip, "marcar como enviado");
        TransitionTo(CustomOrderStatus.Shipped, changedBy);
    }

    /// <summary>Pedido completado y entregado.</summary>
    public void Complete(Guid changedBy)
    {
        EnsureStatus(CustomOrderStatus.Shipped, "completar pedido");
        TransitionTo(CustomOrderStatus.Completed, changedBy);
    }

    /// <summary>Cancela el pedido. No válido una vez en producción (InPrinting en adelante).</summary>
    public void Cancel(Guid changedBy, string reason)
    {
        var nonCancellableStatuses = new[]
        {
            CustomOrderStatus.InPrinting,
            CustomOrderStatus.InFinishing,
            CustomOrderStatus.ReadyToShip,
            CustomOrderStatus.Shipped,
            CustomOrderStatus.Completed
        };

        if (nonCancellableStatuses.Contains(Status))
            throw new DomainException(
                "No se puede cancelar un pedido que ya está en producción o completado.",
                "CUSTOM_ORDER_CANNOT_CANCEL");

        TransitionTo(CustomOrderStatus.Cancelled, changedBy, reason);
    }

    // ─────────────────────────────────────────────
    // Helpers privados
    // ─────────────────────────────────────────────

    private void TransitionTo(CustomOrderStatus newStatus, Guid changedBy, string? notes = null)
    {
        Status = newStatus;
        _statusHistory.Add(new CustomOrderStatusHistory
        {
            CustomOrderId = Id,
            Status = newStatus,
            Notes = notes,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow
        });
    }

    private void EnsureStatus(CustomOrderStatus expected, string action)
    {
        if (Status != expected)
            throw new DomainException(
                $"No se puede {action} en un pedido con estado '{Status}'.",
                "CUSTOM_ORDER_INVALID_STATUS");
    }

    /// <summary>El pedido solo es editable por el cliente en los estados iniciales.</summary>
    private void EnsureEditableStatus()
    {
        if (Status is not (CustomOrderStatus.PendingReview or CustomOrderStatus.UnderReview))
            throw new DomainException(
                "El pedido no puede ser editado en su estado actual.",
                "CUSTOM_ORDER_NOT_EDITABLE");
    }

    private static string GenerateOrderNumber() =>
        $"CUS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}