using DDJ.Domain.Common;
using DDJ.Domain.Entities.Customers;
using DDJ.Domain.Entities.CustomOrders;
using DDJ.Domain.Entities.Orders;

namespace DDJ.Domain.Entities.Shipping;

public class ShippingAddress : AuditableEntity
{
    public Guid CustomerId { get; private set; }
    public string FullName { get; private set; } = default!;
    public string AddressLine1 { get; private set; } = default!;
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string? Phone { get; private set; }
    public bool IsDefault { get; private set; }

    // Propiedades de navegación
    public Customer Customer { get; private set; } = default!;
    public ICollection<Order> Orders { get; private set; } = [];
    public ICollection<CustomOrder> CustomOrders { get; private set; } = [];
}