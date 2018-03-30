using System;

namespace Ajakka.Alerting.Descriptors{
    public class IsRequiredAttribute:Attribute{
        public bool IsRequired {get;set;}
        public IsRequiredAttribute():this(false){}

        public IsRequiredAttribute(bool isRequired){
            IsRequired = isRequired;
        }
    }
}