using System.Runtime.Serialization;

namespace Ajakka.Collector{
    [DataContract]
    public class VendorDescriptor{
        [DataMember]
        public string Name{get;set;}
        public string OUI{get;set;}
    }
}