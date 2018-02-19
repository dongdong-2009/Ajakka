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

        protected override string ProcessRequest(dynamic request){
            switch(request.FunctionName){
                case "GetLatest":
                    var endpoints = dal.GetEndpoints(request.PageNumber, request.PageSize);
                    var res1 = new DALServerResponse<EndpointDescriptor[]>{
                        Content = endpoints
                    };
                    return SerializeResponse<DALServerResponse<EndpointDescriptor[]>>(res1);

                case "GetDhcpEndpointPageCount":
                    var res2 = new DALServerResponse<int>{
                        Content = dal.GetDhcpEndpointPageCount(request.PageSize)
                    };
                    return SerializeResponse<DALServerResponse<int>>(res2);

                default:
                    throw new InvalidOperationException("Function name not found: " +request.FunctionName);
            }
        }
    }
}