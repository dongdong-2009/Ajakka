namespace Ajakka.Collector{
    public interface ICollectorDAL{
        void StoreDhcpEndpoint(string mac, string ip, string hostname);
    }
}