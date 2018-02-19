using Ajakka.Collector;
using System;
using System.Collections.Generic;

namespace Ajakka.CollectorMock{
    class DAL:ICollectorDAL{
        readonly List<EndpointDescriptor> endpoints = new List<EndpointDescriptor>();
        public DAL(){
            endpoints.AddRange(new[]{
                new EndpointDescriptor{
                    DeviceIpAddress = "192.168.1.2",
                    DeviceMacAddress = "5a3e1e223456",
                    DeviceName = "test-rasp-1",
                    TimeStamp = DateTime.UtcNow
                },
                new EndpointDescriptor{
                    DeviceIpAddress = "192.168.1.3",
                    DeviceMacAddress = "5aea1e223456",
                    DeviceName = "test-laptop-2",
                    TimeStamp = DateTime.UtcNow
                },
                new EndpointDescriptor{
                    DeviceIpAddress = "192.168.1.3",
                    DeviceMacAddress = "5ffa1e223456",
                    DeviceName = "test-router-2",
                    TimeStamp = DateTime.UtcNow
                }

            });
        }

        public void StoreDhcpEndpoint(string mac, string ip, string hostname, DateTime timestamp){

        }

        public EndpointDescriptor[] GetEndpoints(int pageNumber, int pageSize){
            
            return new EndpointDescriptor[]{};
        }

        public int GetDhcpEndpointPageCount(int pageSize){
            return 0;
        }
    }
}