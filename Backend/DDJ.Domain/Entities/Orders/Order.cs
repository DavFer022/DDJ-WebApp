using DDJ.Domain.Common;
using DDJ.Domain.Entities.Customers;
using DDJ.Domain.Entities.Shipping;
using DDJ.Domain.Enums;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Orders;

/// <summary>
/// Aggregate root del pedido estándar.
/// Encapsula el ciclo de vida completo: creación → confirmación de pago → cancelación.
/// En MVP el pago se confirma manualmente por el admin (sin pasarela integrada).
/// </summary>
public class Order : AuditableEntity
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public string? PaymentMethod { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Guid ShippingAddressId { get; private set; }
    public Guid BillingAddressId { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Propiedades de navegación
    public Customer Customer { get; private set; } = default!;
    public ShippingAddress ShippingAddress { get; private set; } = default!;
    public BillingAddress BillingAddress { get; private set; } = default!;

    // Constructor privado – EF Core
    private Order() { }

    // ─────────────────────────────────────────────
    // Factory method
    // ─────────────────────────────────────────────

    /// <summary>
    /// Crea un pedido en estado PendingPayment.
    /// Los ítems deben agregarse mediante AddItem() antes de persistir.
    /// </summary>
    public static Order Create(
        Guid customerId,
        Guid shippingAddressId,
        Guid billingAddressId,
        string? notes = null)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El customerId es requerido.", "ORDER_MISSING_CUSTOMER");
        if (shippingAddressId == Guid.Empty)
            throw new DomainException("La dirección de envío es requerida.", "ORDER_MISSING_SHIPPING");
        if (billingAddressId == Guid.Empty)
            throw new DomainException("La dirección de facturación es requerida.", "ORDER_MISSING_BILLING");

        return new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            ShippingAddressId = shippingAddressId,
            BillingAddressId = billingAddressId,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            TotalAmount = 0,
            Notes = notes
        };
    }

    // ─────────────────────────────────────────────
    // Comportamientos del dominio
    // ─────────────────────────────────────────────

    /// <summary>
    /// Agrega un ítem al pedido. Solo válido mientras el pedido esté en Pending.
    /// Si la variante ya existe, suma la cantidad.
    /// </summary>
    public void AddItem(Guid productVariantId, int quantity, decimal unitPrice)
    {
        EnsureStatus(OrderStatus.Pending, "agregar ítems");

        var existing = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(OrderItem.Create(Id, productVariantId, quantity, unitPrice));

        RecalculateTotal();
    }

    /// <summary>
    /// Confirma el pago manualmente (admin MVP).
    /// Transiciona: PendingPayment → Confirmed.
    /// </summary>
    public void ConfirmPayment(string paymentMethod)
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("No se puede confirmar un pedido cancelado.", "ORDER_ALREADY_CANCELLED");
        if (PaymentStatus == PaymentStatus.Confirmed)
            throw new DomainException("El pago ya fue confirmado.", "ORDER_ALREADY_PAID");
        if (!_items.Any())
            throw new DomainException("El pedido no tiene ítems.", "ORDER_NO_ITEMS");
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new DomainException("El método de pago es requerido.", "ORDER_MISSING_PAYMENT_METHOD");

        PaymentStatus = PaymentStatus.Confirmed;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Confirmed;
    }

    /// <summary>
    /// Avanza el pedido a Processing (preparando envío).
    /// Solo válido desde Confirmed.
    /// </summary>
    public void StartProcessing()
    {
        EnsureStatus(OrderStatus.Confirmed, "iniciar procesamiento");
        Status = OrderStatus.Processing;
    }

    /// <summary>
    /// Marca el pedido como Shipped.
    /// Solo válido desde Processing.
    /// </summary>
    public void MarkAsShipped()
    {
        EnsureStatus(OrderStatus.Processing, "marcar como enviado");
        Status = OrderStatus.Shipped;
    }

    /// <summary>
    /// Marca el pedido como Delivered.
    /// Solo válido desde Shipped.
    /// </summary>
    public void MarkAsDelivered()
    {
        EnsureStatus(OrderStatus.Shipped, "marcar como entregado");
        Status = OrderStatus.Delivered;
    }

    /// <summary>
    /// Cancela el pedido. No se puede cancelar si ya fue enviado o entregado.
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new DomainException(
                "No se puede cancelar un pedido que ya fue enviado o entregado.",
                "ORDER_CANNOT_CANCEL");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("El pedido ya está cancelado.", "ORDER_ALREADY_CANCELLED");

        Status = OrderStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason;
    }

    // ─────────────────────────────────────────────
    // Lógica privada
    // ─────────────────────────────────────────────

    private void RecalculateTotal() =>
        TotalAmount = _items.Sum(i => i.Subtotal);

    private void EnsureStatus(OrderStatus expected, string action)
    {
        if (Status != expected)
            throw new DomainException(
                $"No se puede {action} en un pedido con estado '{Status}'.",
                "ORDER_INVALID_STATUS");
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}