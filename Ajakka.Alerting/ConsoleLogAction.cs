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
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now.ToString(TimestampFormat) + " : " + alertMessage);
            Console.ForegroundColor = currentColor;
        }

        public override void Initialize()
        {
            if(!string.IsNullOrEmpty(Configuration)){
                TimestampFormat = Configuration;
            }
        }
    }
}