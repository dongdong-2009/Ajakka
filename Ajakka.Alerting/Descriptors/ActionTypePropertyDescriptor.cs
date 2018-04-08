using System;
using System.Runtime.Serialization;

namespace Ajakka.Alerting.Descriptors{
    [DataContract]
    public class ActionTypePropertyDescriptor{
        [DataMember]
        public string Name {get;set;}
        [DataMember]
        public string Type {get;set;}
        [DataMember]
        public string DisplayName {get;set;}

        [DataMember]
        public bool IsRequired{get;set;}

        [DataMember]
        public string Hint {get;set;}
        [DataMember]
        public string HintUrl {get;set;}
        public ActionTypePropertyDescriptor(){
            
        }

        public ActionTypePropertyDescriptor(string name, string displayName, string type, bool isRequired=false, string hint = "", string hintUrl = ""){
            Name = name;
            DisplayName = displayName;
            Type = type;
            IsRequired = isRequired;
            Hint = hint;
            HintUrl = hintUrl;
        }
    }
}