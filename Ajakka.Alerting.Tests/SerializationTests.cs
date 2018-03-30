using System.IO;
using System.Runtime.Serialization.Json;
using Xunit;

namespace Ajakka.Alerting.Tests{
    public class SerializationTests{
        [Fact]
        public void CanSerializeLogToFileAction(){
            var action = AlertActionFactory.Create("action1", "Ajakka.Alerting.LogToFileAction","{TimestampFormat:\"MM\"}");
            var serializer = new DataContractJsonSerializer(typeof(AlertActionBase));
            using(var stream = new MemoryStream( )){
                serializer.WriteObject(stream, action);
            }
        }

        [Fact]
         public void CanSerializeConsoleLogAction(){
            var action = AlertActionFactory.Create("action1", "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}");
            var serializer = new DataContractJsonSerializer(typeof(AlertActionBase));
            using(var stream = new MemoryStream( )){
                serializer.WriteObject(stream, action);
            }
        }

        [Fact]
         public void CanSerializeHttpRequestAction(){
            var action = AlertActionFactory.Create("action1", "Ajakka.Alerting.HttpRequestAlertAction","{TimestampFormat:\"MM\"}");
            var serializer = new DataContractJsonSerializer(typeof(AlertActionBase));
            using(var stream = new MemoryStream( )){
                serializer.WriteObject(stream, action);
            }
        }
    }
}