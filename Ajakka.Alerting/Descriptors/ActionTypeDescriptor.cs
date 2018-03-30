using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ajakka.Alerting.Descriptors{
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

        public ActionTypeDescriptor(Type type, ActionTypePropertyDescriptor[] properties){
            var attributes = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if(attributes.Length == 0){
                throw new InvalidOperationException("The type is missing DisplayName attribute");
            }
            Name = ((DisplayNameAttribute)attributes[0]).DisplayName;
            TypeName = type.FullName;
            this.properties.AddRange(properties);
        }
    }
}