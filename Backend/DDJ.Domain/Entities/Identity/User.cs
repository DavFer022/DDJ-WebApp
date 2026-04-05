using DDJ.Domain.Common;
using DDJ.Domain.Entities.Customers;

namespace DDJ.Domain.Entities.Identity;

public class User : AuditableEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    // Encapsulamos la relación Many-to-Many con Role
    private readonly List<Role> _roles = [];
    public virtual IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    // Propiedad de navegación
    public Customer? Customer { get; private set; }

    public void AddRole(Role role)
    {
       if (_roles.Any(r => r.Id == role.Id)) 
            throw new InvalidOperationException("El usuario ya tiene este rol");

        _roles.Add(role);
    }

    public void RemoveRole(Guid roleId)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role != null) _roles.Remove(role);
    }
}