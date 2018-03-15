using System;

namespace Ajakka.Alerting{
    public class ActionTypeDescriptor{
        public string Name {get;set;}
        public string TypeName {get;set;}

        public ActionTypeDescriptor(){

        }

        public ActionTypeDescriptor(string name, Type type){
            Name = name;
            TypeName = type.FullName;
        }
    }
}