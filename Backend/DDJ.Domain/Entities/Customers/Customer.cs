using DDJ.Domain.Common;
using DDJ.Domain.Entities.Identity;
using DDJ.Domain.Entities.Orders;
using DDJ.Domain.Entities.Shipping;

namespace DDJ.Domain.Entities.Customers;

public class Customer : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Preferences { get; private set; }

    // Propiedades de navegación
    public User User { get; private set; } = default!;
    public ICollection<Order> Orders { get; private set; } = [];
    public ICollection<BillingAddress> BillingAddresses { get; private set; } = [];
    public ICollection<ShippingAddress> ShippingAddresses { get; private set; } = [];
}
