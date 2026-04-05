using DDJ.Domain.Common;
using DDJ.Domain.Entities.Customers;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Orders;

/// <summary>
/// Aggregate root del carrito de compras.
/// Soporta carrito anónimo (CustomerId = null) y autenticado.
/// Toda mutación de ítems ocurre exclusivamente a través de este aggregate.
/// </summary>
public class Cart : AuditableEntity
{
    /// <summary>Null cuando el carrito es anónimo (usuario no autenticado).</summary>
    public Guid? CustomerId { get; private set; }

    /// <summary>Clave de sesión para identificar carritos anónimos en el frontend.</summary>
    public string SessionId { get; private set; } = default!;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    /// <summary>Número total de unidades en el carrito.</summary>
    public int TotalItems => _items.Sum(i => i.Quantity);

    public bool IsAnonymous => CustomerId is null;

    // Propiedades de navegación
    public Customer? Customer { get; private set; }

    // Constructor privado – EF Core
    private Cart() { }

    // ─────────────────────────────────────────────
    // Factory methods
    // ─────────────────────────────────────────────

    /// <summary>Crea un carrito anónimo para un visitante sin sesión.</summary>
    public static Cart CreateAnonymous(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new DomainException("El sessionId es requerido.", "CART_MISSING_SESSION");

        return new Cart { SessionId = sessionId };
    }

    /// <summary>Crea un carrito asociado a un cliente autenticado.</summary>
    public static Cart CreateForCustomer(Guid customerId, string sessionId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El customerId es requerido.", "CART_MISSING_CUSTOMER");

        return new Cart { CustomerId = customerId, SessionId = sessionId };
    }

    // ─────────────────────────────────────────────
    // Comportamientos del dominio
    // ─────────────────────────────────────────────

    /// <summary>
    /// Agrega una variante al carrito. Si ya existe la variante, incrementa la cantidad.
    /// </summary>
    public void AddItem(Guid productVariantId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.", "CART_INVALID_QUANTITY");

        var existing = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
        if (existing is not null)
            existing.UpdateQuantity(existing.Quantity + quantity);
        else
            _items.Add(CartItem.Create(Id, productVariantId, quantity));
    }

    /// <summary>Actualiza la cantidad de una variante existente. Si newQuantity ≤ 0, elimina el ítem.</summary>
    public void UpdateItemQuantity(Guid productVariantId, int newQuantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId)
            ?? throw new DomainException("El ítem no existe en el carrito.", "CART_ITEM_NOT_FOUND");

        if (newQuantity <= 0)
            _items.Remove(item);
        else
            item.UpdateQuantity(newQuantity);
    }

    /// <summary>Elimina una variante del carrito.</summary>
    public void RemoveItem(Guid productVariantId)
    {
        var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId)
            ?? throw new DomainException("El ítem no existe en el carrito.", "CART_ITEM_NOT_FOUND");

        _items.Remove(item);
    }

    /// <summary>Vacía todos los ítems del carrito.</summary>
    public void Clear() => _items.Clear();

    /// <summary>
    /// Fusiona los ítems de un carrito anónimo en este carrito y lo asocia al cliente.
    /// Las cantidades de variantes duplicadas se suman.
    /// </summary>
    public void MergeWith(Cart anonymousCart, Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El customerId es requerido para el merge.", "CART_MISSING_CUSTOMER");

        foreach (var item in anonymousCart.Items)
            AddItem(item.ProductVariantId, item.Quantity);

        CustomerId = customerId;
    }
}
