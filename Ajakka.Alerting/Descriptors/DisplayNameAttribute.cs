using System;

namespace Ajakka.Alerting.Descriptors{
    class DisplayNameAttribute:Attribute{
        public string DisplayName {get;set;}
        public DisplayNameAttribute() : this(""){

        }

        public DisplayNameAttribute(string displayName){
            DisplayName = displayName;
        }
    }
}