namespace DDJ.Domain.Enums;

public enum OrderStatus
{
    /// <summary>Pedido creado, esperando confirmación de pago.</summary>
    Pending = 0,
    /// <summary>Pago confirmado por el admin (MVP) o por la pasarela (V1).</summary>
    Confirmed = 1,
    /// <summary>En preparación para envío.</summary>
    Processing = 2,
    /// <summary>Enviado al cliente.</summary>
    Shipped = 3,
    /// <summary>Entregado al cliente.</summary>
    Delivered = 4,
    /// <summary>Pedido cancelado.</summary>
    Cancelled = 5,
    /// <summary>Pago reembolsado.</summary>
    Refunded = 6
}

public enum CustomOrderStatus
{
    PendingReview = 0,
    UnderReview = 1,
    Quoted = 2,
    AwaitingApproval = 3,
    Approved = 4,
    InDesign = 5,
    InPrinting = 6,
    InFinishing = 7,
    ReadyToShip = 8,
    Shipped = 9,
    Completed = 10,
    Cancelled = 11
}

public enum PaymentStatus
{
    /// <summary>Sin pago registrado.</summary>
    Pending = 0,
    /// <summary>Pago confirmado manualmente por el admin (MVP).</summary>
    Confirmed = 1,
    /// <summary>Pago autorizado por pasarela (V1).</summary>
    Authorized = 2,
    /// <summary>Pago capturado/cobrado por pasarela (V1).</summary>
    Captured = 3,
    /// <summary>Pago rechazado.</summary>
    Failed = 4,
    /// <summary>Pago reembolsado.</summary>
    Refunded = 5,
    /// <summary>Pago parcialmente reembolsado.</summary>
    PartiallyRefunded = 6
}

public enum ShipmentStatus
{
    Preparing = 0,
    Dispatched = 1,
    InTransit = 2,
    OutForDelivery = 3,
    Delivered = 4,
    ReturnedToSender = 5
}
