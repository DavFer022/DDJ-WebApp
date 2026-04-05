using DDJ.Domain.Common;

namespace DDJ.Domain.Entities.Notifications;

/// <summary>
/// Registro de notificaciones enviadas (email u otros canales).
/// Permite seguimiento de entregas y reintento en caso de fallo.
/// </summary>
public class NotificationLog : BaseEntity
{
    public Guid? UserId { get; private set; }               // Null si destinatario sin cuenta
    public string RecipientEmail { get; private set; } = default!;
    public string NotificationType { get; private set; } = default!; // OrderConfirmation, StatusUpdate, etc.
    public string Status { get; private set; } = default!;  // Sent, Failed
    public DateTime? SentAt { get; private set; }
    public string? ErrorMessage { get; private set; }       // Razón del fallo si aplica
}
