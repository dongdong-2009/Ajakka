using System;
using System.IO;
using System.Runtime.Serialization;

namespace Ajakka.Alerting{
    [DataContract]
    public class LogToFileAction : AlertActionBase
    {
        public LogToFileAction(){
            TimestampFormat = "G";
        }

        [DataMember]
        public string FileName{get;set;}
         [DataMember]
        public string TimestampFormat {get;set;}

        public override object Clone()
        {
            return new LogToFileAction{
                Configuration = this.Configuration,
                Name = this.Name,
                FileName = this.FileName,
                Id = Id
            };
        }

        public override void Execute(string alertMessage){
            try{
                using(var writer = new StreamWriter(FileName, true)){
                    writer.WriteLine(DateTime.Now.ToString(TimestampFormat) + " : " + alertMessage);
                }
            }
            catch(Exception ex){
                Console.WriteLine("Could not log to file: " + ex);
            }
        }

        public override void Initialize()
        {
            if(string.IsNullOrEmpty(Configuration))
                return;
            
            var config = ParseConfiguration(new {TimestampFormat = "", FileName = ""});
          
            TimestampFormat = config.TimestampFormat;
            FileName = config.FileName;
            if(string.IsNullOrEmpty(FileName)){
                throw new InvalidOperationException("FileName property is not set");
            }
        }
    }
}