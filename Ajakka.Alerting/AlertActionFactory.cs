using System;

namespace Ajakka.Alerting{
    public class AlertActionFactory{
        public static AlertActionBase Create(string name, string type, string configuration){
            Type t = Type.GetType(type);
            if(t == null){
                throw new ArgumentException("type " + type + " does not exist");
            }
            var alertAction = (AlertActionBase)Activator.CreateInstance(t);
            alertAction.Configuration = configuration;
            alertAction.Name = name;
            return alertAction;
        }

        public static ActionTypeDescriptor[] GetAlertActionTypes(){
            return new ActionTypeDescriptor[]{
                new ActionTypeDescriptor("Log to console", typeof(ConsoleLogAction),new[]{new ActionTypePropertyDescriptor("TimestampFormat","Timestamp format","text")}),
                new ActionTypeDescriptor("Log to file", typeof(LogToFileAction), new[]{
                    new ActionTypePropertyDescriptor("TimestampFormat","Timestamp format","text"),
                    new ActionTypePropertyDescriptor("FileName","File name","text")})
            };
        }
    }
}