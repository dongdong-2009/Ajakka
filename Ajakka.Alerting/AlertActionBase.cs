using System;
using System.Runtime.Serialization;

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

        public virtual void Execute(string alertMessage){

        }

        public abstract void Initialize();
        public abstract object Clone();

        static Type[] GetKnownTypes()
        {
            return new Type[] { typeof(ConsoleLogAction) };
        }
    }
}