using System;
using System.Runtime.Serialization;

namespace Ajakka.Messaging{
    [DataContract]
    public class DeviceDescriptorMessage{
        [DataMember]
        public string DeviceName;
        [DataMember]
        public string DeviceMacAddress;
        [DataMember]
        public string DeviceIpAddress;
        [DataMember]
        public DateTime TimeStamp;
    }
}