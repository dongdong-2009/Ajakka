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
            ConsoleLogAction alertAction = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","MM");
            alertAction.Initialize();
            Assert.Equal("MM", alertAction.TimestampFormat);
            Assert.Equal("log to console",alertAction.Name);
        }

        [Fact]
        public void ShouldFailToCreateConsoleLogAction()
        {
            Assert.Throws<ArgumentException>(()=>{
                ConsoleLogAction alertAction = (ConsoleLogAction)AlertActionFactory.Create("log to console","ConsoleLogAction","MM");
            });
        }

    }
}
