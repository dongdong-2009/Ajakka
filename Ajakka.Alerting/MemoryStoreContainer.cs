using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ajakka.Alerting{
    [DataContract]
    public class MemoryStoreContainer{
        [DataMember]
        public Dictionary<int, AlertActionBase> AlertActions {get;set;}
        [DataMember]
        public int LastId {get;set;}
        [DataMember]
        public Dictionary<Guid, List<int>> RuleToActionMap {get;set;}

       
    }
}