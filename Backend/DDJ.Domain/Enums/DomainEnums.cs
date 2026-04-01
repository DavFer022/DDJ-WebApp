namespace DDJ.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Paid = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
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
    Pending = 0,
    Authorized = 1,
    Captured = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5
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
