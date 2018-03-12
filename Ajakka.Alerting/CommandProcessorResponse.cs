using System.Runtime.Serialization;

namespace Ajakka.Alerting{
     [DataContract]
    public class CommandProcessorResponse<T>{
        [DataMember]
        public bool Error;
        [DataMember]
        public T Content;
        [DataMember]
        public string Message;
    }
}