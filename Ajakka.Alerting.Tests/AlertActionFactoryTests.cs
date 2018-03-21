using System;
using Xunit;
using Ajakka.Alerting;

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
    }
}
