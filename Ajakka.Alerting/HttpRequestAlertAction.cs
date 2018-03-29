using System;
using System.Net;
using System.Runtime.Serialization;

namespace Ajakka.Alerting{
    [DataContract]
    public class HttpRequestAlertAction : AlertActionBase
    {
        [DataMember]
        public string Url {get;set;}

        public HttpRequestAlertAction(){
            Url = "";
        }
        public override void Execute(string alertMessage){
            if(string.IsNullOrEmpty(Url)){
                return;
            }
            try{
                var client = new WebClient();
                client.DownloadData(Url);
            }catch(Exception ex){
                Console.WriteLine("Error executing Http Request alert action: " + ex);
            }
        }

        public override object Clone()
        {
            return new HttpRequestAlertAction{
                Configuration = this.Configuration,
                Name = this.Name,
                Url = this.Url,
                Id = Id
            };
        }

        public override void Initialize()
        {
            if(string.IsNullOrEmpty(Configuration))
                return;
            
            var config = ParseConfiguration(new {Url = ""});
          
            Url = config.Url;
        }
    }
}