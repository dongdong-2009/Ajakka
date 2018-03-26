using System;

namespace Ajakka.Collector{
    
    

    public interface ICollectorDAL{
        void StoreDhcpEndpoint(string mac, string ip, string hostname, DateTime timestamp, string detectedBy);

        EndpointDescriptor[] GetEndpoints(int pageNumber, int pageSize);
        int GetDhcpEndpointPageCount(int pageSize);
    }
}