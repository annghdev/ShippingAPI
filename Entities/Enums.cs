namespace ShippingAPI.Entities;

public enum ShippingStatus
{
    Pending,
    ReadyToPick,
    Picking,
    Picked,
    Storing,
    Delivering,
    Delivered,
    DeliveryFail,
    WaitingToReturn,
    Return,
    Returned,
    Cancelled
}
