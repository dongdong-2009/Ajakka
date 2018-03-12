using System;

namespace Ajakka.Alerting{
    public sealed class ConsoleLogAction : AlertActionBase
    {
        public ConsoleLogAction(){
            TimestampFormat = "G";
        }

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
            Console.WriteLine(DateTime.Now.ToString(TimestampFormat) + " : " + alertMessage);
        }

        public override void Initialize()
        {
            if(!string.IsNullOrEmpty(Configuration)){
                TimestampFormat = Configuration;
            }
        }
    }
}