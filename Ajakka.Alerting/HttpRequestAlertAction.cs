using System;
using System.Net;
using System.Runtime.Serialization;
using Ajakka.Alerting.Descriptors;

namespace Ajakka.Alerting{
    [DataContract]
    [DisplayName("Send HTTP GET request")]
    public class HttpRequestAlertAction : AlertActionBase
    {
        [DataMember]
        [DisplayName("URL")]
        [PropertyType("text")]
        [IsRequired(true)]
        [PropertyHint("Specify the full URL the request will be made to. Click to get more help.","https://github.com/pilvikala/Ajakka#alerting")]
        public string Url {get;set;}

        public HttpRequestAlertAction(){
            Url = "";
        }
        public override void Execute(dynamic data){
            if(string.IsNullOrEmpty(Url)){
                return;
            }
            var parser = new MacroParser();
            var requestUrl = parser.Parse(Url, data);
            try{
                var client = new WebClient();
                client.DownloadData(requestUrl);
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