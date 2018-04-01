using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

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

        public ActionTypeDescriptor(Type type){
            var attributes = type.GetCustomAttributes(false);
            Name = ((DisplayNameAttribute)attributes.First((attr)=>{
                return attr is DisplayNameAttribute;    
            })).DisplayName;
            TypeName = type.FullName;
            var props = type.GetProperties(BindingFlags.Instance|BindingFlags.Public);
            foreach(var p in props){
                attributes = p.GetCustomAttributes(false);
                var propDisplayNameAttribute = (DisplayNameAttribute)attributes.FirstOrDefault((attr)=>{
                    return attr is DisplayNameAttribute;
                });
                if(propDisplayNameAttribute == null)
                    continue;
                var propTypeAttribute = (PropertyTypeAttribute)attributes.First((attr)=>{
                    return attr is PropertyTypeAttribute;
                });
                var isRequiredAttribute = (IsRequiredAttribute)attributes.FirstOrDefault((attr)=>{
                    return attr is IsRequiredAttribute;
                });
                this.properties.Add(new ActionTypePropertyDescriptor(p.Name,
                    propDisplayNameAttribute.DisplayName,
                    propTypeAttribute.Type,
                    isRequiredAttribute == null ? false : isRequiredAttribute.IsRequired));
            }
        }
    }
}