using System;
using System.Runtime.Serialization;

namespace Ajakka.Collector{
    [DataContract]
    public class DALServerResponse<T>{
        [DataMember]
        public bool Error;
        [DataMember]
        public T Content;
        [DataMember]
        public string Message;
    }
}