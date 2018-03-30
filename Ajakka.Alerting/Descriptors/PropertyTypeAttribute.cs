using System;

namespace Ajakka.Alerting.Descriptors{
    public class PropertyTypeAttribute:Attribute{
        public string Type {get;set;}
        public PropertyTypeAttribute():this(""){}

        public PropertyTypeAttribute(string propertyType){
            Type = propertyType;
        }
    }
}