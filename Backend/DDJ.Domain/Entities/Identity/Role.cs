using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Identity;

public class Role: AuditableEntity
{
    private readonly List<Permission> _permissions = [];

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    //Propiedad de navegación
    public IReadOnlyList<User> Users { get; private set; } = [];

    public virtual IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public Role(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));
        
        Name = name;
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id)) return;
        _permissions.Add(permission);
    }
}


