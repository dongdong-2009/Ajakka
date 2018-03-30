using System;
using Xunit;
using Ajakka.Alerting;
using System.Linq;

namespace Ajakka.Alerting.Tests
{
    public class AlertActionFactoryTests
    {
        [Fact]
        public void ShouldCreateConsoleLogAction()
        {
            ConsoleLogAction alertAction = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{\"TimestampFormat\":\"MM\"}");
            alertAction.Initialize();
            Assert.Equal("MM", alertAction.TimestampFormat);
            Assert.Equal("log to console",alertAction.Name);
        }

        [Fact]
        public void ShouldFailToCreateConsoleLogAction()
        {
            Assert.Throws<ArgumentException>(()=>{
                ConsoleLogAction alertAction = (ConsoleLogAction)AlertActionFactory.Create("log to console","ConsoleLogAction","{\"TimestampFormat\":\"MM\"}");
            });
        }

          [Fact]
        public void ShouldFailToCreateLogToFileAction()
        {
            LogToFileAction alertAction = (LogToFileAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.LogToFileAction","{\"TimestampFormat\":\"MM\"}");
                
            Assert.Throws<InvalidOperationException>(()=>{
                alertAction.Initialize();
            });
        }

        [Fact]
        public void ShouldCreateLogToFileAction()
        {
            LogToFileAction alertAction = (LogToFileAction)AlertActionFactory.Create("log to file","Ajakka.Alerting.LogToFileAction","{\"TimestampFormat\":\"MM\",\"FileName\":\"alert.log\"}");
            alertAction.Initialize();
            Assert.Equal("MM", alertAction.TimestampFormat);
            Assert.Equal("log to file",alertAction.Name);
            Assert.Equal("alert.log",alertAction.FileName);
        }

        [Fact]
        public void BugFix_ShouldNotLogOutsideCurrentDirectory(){
            LogToFileAction alertAction = (LogToFileAction)AlertActionFactory.Create("log to file","Ajakka.Alerting.LogToFileAction","{\"TimestampFormat\":\"MM\",\"FileName\":\"c:\\\\alert.log\"}");
            var store = ActionStoreFactory.GetActionStore();
            var added = store.AddAction(alertAction);
            var actual = store.GetAction(added.Id);
            Assert.Equal("alert.log",((LogToFileAction)actual).FileName);
        }

        [Fact]
        public void ShouldReturnActionTypes(){
            var alertActionTypes = AlertActionFactory.GetAlertActionTypes();
            ValidateLogToFileActionType(alertActionTypes.First((actionType)=>{
                return actionType.TypeName == "Ajakka.Alerting.LogToFileAction";
            }));
            ValidateHttpRequestAlertActionType(alertActionTypes.First((actionType)=>{
                return actionType.TypeName == "Ajakka.Alerting.HttpRequestAlertAction";
            }));
            ValidateConsoleLogActionType(alertActionTypes.First((actionType)=>{
                return actionType.TypeName == "Ajakka.Alerting.ConsoleLogAction";
            }));
        }

        private static void ValidateConsoleLogActionType(ActionTypeDescriptor actionType){
            Assert.NotNull(actionType);
            Assert.Equal("Log to console",actionType.Name);
            ValidateProperty(actionType.Properties.First((prop)=>{return prop.Name == "TimestampFormat";} ),
            "Timestamp format","text",false);
            Assert.Equal(1,actionType.Properties.Length);
        }
        private static void ValidateHttpRequestAlertActionType(ActionTypeDescriptor actionType){
            Assert.NotNull(actionType);
            Assert.Equal("Send HTTP GET request",actionType.Name);
            ValidateProperty(actionType.Properties.First((prop)=>{return prop.Name == "Url";} ),
            "URL","text",false);
            Assert.Equal(1,actionType.Properties.Length);
        }
        private static void ValidateLogToFileActionType(ActionTypeDescriptor actionType){
            Assert.NotNull(actionType);
            Assert.Equal("Log to file",actionType.Name);
            ValidateProperty(actionType.Properties.First((prop)=>{return prop.Name == "TimestampFormat";} ),
            "Timestamp format","text",false);
             ValidateProperty(actionType.Properties.First((prop)=>{return prop.Name == "FileName";} ),
            "File name","text",true);
            Assert.Equal(2,actionType.Properties.Length);
        }

        private static void ValidateProperty(ActionTypePropertyDescriptor prop, string expectedDisplayName, string expectedType, bool expectedIsRequired){
            Assert.Equal(expectedDisplayName, prop.DisplayName);
            Assert.Equal(expectedIsRequired, prop.IsRequired);
            Assert.Equal(expectedType, prop.Type);
        }
    }
}
