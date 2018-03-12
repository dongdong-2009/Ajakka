using System;

namespace Ajakka.Alerting{
    public abstract class AlertActionBase:ICloneable{
        public string Configuration {get;set;}
        public int Id {get;set;}
        public string Name {get;set;}

        public virtual void Execute(string alertMessage){

        }

        public abstract void Initialize();
        public abstract object Clone();
    }
}