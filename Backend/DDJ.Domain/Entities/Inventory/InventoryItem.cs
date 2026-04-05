using DDJ.Domain.Common;
using DDJ.Domain.Entities.Catalog;
using DDJ.Domain.Exceptions;

namespace DDJ.Domain.Entities.Inventory;

/// <summary>
/// Aggregate root del inventario por variante de producto.
/// Gestiona disponibilidad, reservas y niveles de reorden.
/// Relación 1-to-1 con ProductVariant.
/// </summary>
public class InventoryItem : AuditableEntity
{
    public Guid ProductVariantId { get; private set; }
    public int QuantityAvailable { get; private set; }
    public int QuantityReserved { get; private set; }
    public int ReorderLevel { get; private set; } = 5;

    /// <summary>Unidades disponibles para venta (descontando reservas activas).</summary>
    public int EffectiveStock => QuantityAvailable - QuantityReserved;

    /// <summary>True si el stock efectivo está en o por debajo del nivel de reorden.</summary>
    public bool IsLowStock => EffectiveStock <= ReorderLevel;

    /// <summary>True si no hay stock disponible para nuevas ventas.</summary>
    public bool IsOutOfStock => EffectiveStock <= 0;

    // Propiedades de navegación
    public ProductVariant ProductVariant { get; private set; } = default!;

    // Constructor privado – EF Core
    private InventoryItem() { }

    // ─────────────────────────────────────────────
    // Factory method
    // ─────────────────────────────────────────────

    /// <summary>Inicializa el inventario de una variante con stock inicial.</summary>
    public static InventoryItem Create(Guid productVariantId, int initialStock = 0, int reorderLevel = 5)
    {
        if (productVariantId == Guid.Empty)
            throw new DomainException("El productVariantId es requerido.", "INVENTORY_MISSING_VARIANT");
        if (initialStock < 0)
            throw new DomainException("El stock inicial no puede ser negativo.", "INVENTORY_NEGATIVE_STOCK");
        if (reorderLevel < 0)
            throw new DomainException("El nivel de reorden no puede ser negativo.", "INVENTORY_NEGATIVE_REORDER");

        return new InventoryItem
        {
            ProductVariantId = productVariantId,
            QuantityAvailable = initialStock,
            QuantityReserved = 0,
            ReorderLevel = reorderLevel
        };
    }

    // ─────────────────────────────────────────────
    // Comportamientos del dominio
    // ─────────────────────────────────────────────

    /// <summary>
    /// Reserva unidades para un pedido en proceso.
    /// Las unidades reservadas siguen contabilizadas en QuantityAvailable
    /// pero se descuentan del EffectiveStock.
    /// </summary>
    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a reservar debe ser mayor a cero.", "INVENTORY_INVALID_QUANTITY");
        if (quantity > EffectiveStock)
            throw new DomainException(
                $"Stock insuficiente. Disponible: {EffectiveStock}, solicitado: {quantity}.",
                "INVENTORY_INSUFFICIENT_STOCK");

        QuantityReserved += quantity;
    }

    /// <summary>
    /// Libera reserva sin descontar stock (pedido cancelado).
    /// </summary>
    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a liberar debe ser mayor a cero.", "INVENTORY_INVALID_QUANTITY");
        if (quantity > QuantityReserved)
            throw new DomainException(
                $"No hay suficientes unidades reservadas. Reservadas: {QuantityReserved}.",
                "INVENTORY_INSUFFICIENT_RESERVATION");

        QuantityReserved -= quantity;
    }

    /// <summary>
    /// Confirma el despacho: descuenta las unidades tanto de disponibles como de reservadas.
    /// Llamar cuando el pedido es enviado físicamente.
    /// </summary>
    public void Dispatch(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a despachar debe ser mayor a cero.", "INVENTORY_INVALID_QUANTITY");
        if (quantity > QuantityReserved)
            throw new DomainException(
                $"No hay suficientes unidades reservadas para despachar. Reservadas: {QuantityReserved}.",
                "INVENTORY_INSUFFICIENT_RESERVATION");

        QuantityAvailable -= quantity;
        QuantityReserved -= quantity;
    }

    /// <summary>
    /// Agrega stock (recepción de mercancía o ajuste manual por admin).
    /// </summary>
    public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a reabastecer debe ser mayor a cero.", "INVENTORY_INVALID_QUANTITY");

        QuantityAvailable += quantity;
    }

    /// <summary>
    /// Ajuste manual de stock (inventario físico, corrección de errores).
    /// </summary>
    public void AdjustStock(int newQuantity)
    {
        if (newQuantity < 0)
            throw new DomainException("El stock no puede ser negativo.", "INVENTORY_NEGATIVE_STOCK");
        if (newQuantity < QuantityReserved)
            throw new DomainException(
                $"El nuevo stock ({newQuantity}) no puede ser menor a las reservas activas ({QuantityReserved}).",
                "INVENTORY_STOCK_BELOW_RESERVATIONS");

        QuantityAvailable = newQuantity;
    }

    /// <summary>Actualiza el nivel de reorden para alertas de bajo stock.</summary>
    public void UpdateReorderLevel(int newLevel)
    {
        if (newLevel < 0)
            throw new DomainException("El nivel de reorden no puede ser negativo.", "INVENTORY_NEGATIVE_REORDER");

        ReorderLevel = newLevel;
    }
}