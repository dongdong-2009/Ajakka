using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ajakka.Alerting{
    public class ActionTypeDescriptor{
        public string Name {get;set;}
        public string TypeName {get;set;}
        private readonly List<ActionTypePropertyDescriptor> properties = new List<ActionTypePropertyDescriptor>();
        
        public ActionTypePropertyDescriptor[] Properties {get{return properties.ToArray();}}
        public ActionTypeDescriptor(){

        }

        public ActionTypeDescriptor(string name, Type type, ActionTypePropertyDescriptor[] properties){
            Name = name;
            TypeName = type.FullName;
            this.properties.AddRange(properties);
        }
    }

    [DataContract]
    public class ActionTypePropertyDescriptor{
        [DataMember]
        public string Name {get;set;}
        [DataMember]
        public string Type {get;set;}
        [DataMember]
        public string DisplayName {get;set;}

        public ActionTypePropertyDescriptor(){
            
        }
        public ActionTypePropertyDescriptor(string name,string displayName, string type){
            Name = name;
            DisplayName = displayName;
            Type = type;
        }
    }
}