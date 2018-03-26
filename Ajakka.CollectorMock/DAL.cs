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
                    TimeStamp = DateTime.UtcNow,
                    DetectedBy = "sensor-1"
                },
                new EndpointDescriptor{
                    DeviceIpAddress = "192.168.1.3",
                    DeviceMacAddress = "5aea1e223456",
                    DeviceName = "test-laptop-2",
                    TimeStamp = DateTime.UtcNow,
                    DetectedBy = "sensor-1"
                },
                new EndpointDescriptor{
                    DeviceIpAddress = "192.168.1.3",
                    DeviceMacAddress = "5ffa1e223456",
                    DeviceName = "test-router-2",
                    TimeStamp = DateTime.UtcNow,
                    DetectedBy = "sensor-2"
                }

            });
        }

        public void StoreDhcpEndpoint(string mac, string ip, string hostname, DateTime timestamp, string detectedBy){
            endpoints.Add(new EndpointDescriptor{
                DeviceIpAddress = ip,
                DeviceMacAddress = mac,
                DeviceName = hostname,
                TimeStamp = timestamp,
                DetectedBy = detectedBy
            });
        }

        public EndpointDescriptor[] GetEndpoints(int pageNumber, int pageSize){
            
            return endpoints.ToArray();
        }

        public int GetDhcpEndpointPageCount(int pageSize){
            return 1;
        }
    }
}