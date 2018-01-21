namespace Ajakka.Net{
    public enum DhcpMessageType : byte{
        Unknown = 0,
        Discover = 1,
        Offer = 2,
        Request = 3,
        Decline = 4,
        Ack = 5,
        Nak = 6,
        Release = 7,
        Inform = 8
    }
}