using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ajakka.Alerting.Descriptors;

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

        public static Type[] GetAlertActionTypes(){
            var types = Assembly.GetExecutingAssembly().GetExportedTypes();
            return(from Type t in types where
             t.BaseType != null &&
             t.BaseType.FullName == "Ajakka.Alerting.AlertActionBase" 
             select t).ToArray();
        }

        public static ActionTypeDescriptor[] GetAlertActionTypeDescriptors(){
            return new ActionTypeDescriptor[]{
                new ActionTypeDescriptor(typeof(ConsoleLogAction),new[]{new ActionTypePropertyDescriptor("TimestampFormat","Timestamp format","text")}),
                new ActionTypeDescriptor(typeof(LogToFileAction), new[]{
                    new ActionTypePropertyDescriptor("TimestampFormat","Timestamp format","text"),
                    new ActionTypePropertyDescriptor("FileName","File name","text", true)}),
                new ActionTypeDescriptor(typeof(HttpRequestAlertAction),new[]{new ActionTypePropertyDescriptor("Url","URL","text")}),
            };
        }
    }
}