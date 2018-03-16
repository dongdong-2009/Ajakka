using System;
using System.Runtime.Serialization;

namespace Ajakka.Alerting{
    
    [DataContract]
    public sealed class ConsoleLogAction : AlertActionBase
    {
        public ConsoleLogAction(){
            TimestampFormat = "G";
        }

        [DataMember]
        public string TimestampFormat {get;set;}

        public override object Clone()
        {
            return new ConsoleLogAction{
                Configuration = this.Configuration,
                Name = this.Name,
                TimestampFormat = this.TimestampFormat,
                Id = Id
            };
        }

        public override void Execute(string alertMessage){
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now.ToString(TimestampFormat) + " : " + alertMessage);
            Console.ForegroundColor = currentColor;
        }

        public override void Initialize()
        {
            if(string.IsNullOrEmpty(Configuration))
                return;
            
            var config = ParseConfiguration(new {TimestampFormat = ""});
            TimestampFormat = config.TimestampFormat;
        }
    }
}