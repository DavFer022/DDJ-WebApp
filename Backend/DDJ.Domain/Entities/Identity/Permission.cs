using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Identity;

public class Permission : AuditableEntity
{
    public Permission(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));
        
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
}