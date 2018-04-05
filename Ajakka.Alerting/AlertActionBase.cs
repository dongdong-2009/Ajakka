using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Ajakka.Alerting{
    [DataContract]
    [KnownType("GetKnownTypes")]
    public abstract class AlertActionBase:ICloneable{
        [DataMember]
        public string Configuration {get;set;}
        [DataMember]
        public int Id {get;set;}
        [DataMember]
        public string Name {get;set;}

        [DataMember]
        public string TypeName {get{
            return this.GetType().FullName;
        }
            set{}
        }

        public virtual void Execute(dynamic data){

        }

        protected static string GetAlertMessage(dynamic data){
            return "Blacklist match: " + data.Mac + "/" + data.Ip + "/" + data.Name;
        }

        public abstract void Initialize();
        public abstract object Clone();

        static Type[] GetKnownTypes()
        {
            return AlertActionFactory.GetAlertActionTypes();
        }

        protected dynamic ParseConfiguration(dynamic definition){
            return JsonConvert.DeserializeAnonymousType(Configuration, definition);
        }
    }
}