using Ajakka.Collector;
using System;

namespace Ajakka.CollectorMock{
    class DALServerMock : DALServer
    {
        ICollectorDAL dal;

        public DALServerMock(ICollectorConfiguration configuration, ICollectorDAL dal) : base(configuration, dal)
        {
            this.dal = dal;
        }

         protected override DALServerResponse<int> GetDhcpEndpointPageCount(int pageSize){
            return WrapResponse<int>(dal.GetDhcpEndpointPageCount(pageSize));
        }

        protected override DALServerResponse<EndpointDescriptor[]> GetLatest(int pageNumber, int pageSize){
            return WrapResponse<EndpointDescriptor[]>(dal.GetEndpoints(pageNumber, pageSize));
           
        }

    }
}