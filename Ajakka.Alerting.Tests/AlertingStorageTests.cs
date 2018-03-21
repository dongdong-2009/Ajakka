using System.Collections.Generic;
using Xunit;

namespace Ajakka.Alerting.Tests{
    public class AlertingStorageTests{

        [Fact]
        void ShouldSaveAndLoadActions(){
            var actionStore = ActionStoreFactory.GetActionStore();
            List<AlertActionBase> actions = new List<AlertActionBase>();
            for(int i =0; i < 50; i++){
                var action = AlertActionFactory.Create("action " + i, "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}");
                action.Initialize();
                var added = actionStore.AddAction(action);
                action.Id = added.Id;
                actions.Add(action);
            }
            for(int i =50; i < 100; i++){
                var action = AlertActionFactory.Create("action " + i, "Ajakka.Alerting.LogToFileAction","{TimestampFormat:\"MM\",FileName:\"log.log\"}");
                action.Initialize();
                var added = actionStore.AddAction(action);
                action.Id = added.Id;
                actions.Add(action);
            }
            
            ((IAlertingStorage)actionStore).Save("alerts_test");
            var actionStore2 = ActionStoreFactory.GetActionStore();
            var action2 = AlertActionFactory.Create("action e", "Ajakka.Alerting.LogToFileAction","{TimestampFormat:\"MM\",FileName:\"log.log\"}");
            actionStore2.AddAction(action2);

            ((IAlertingStorage)actionStore2).Load("alerts_test");
        

            foreach(var expected in actions){
                var actual = actionStore2.GetAction(expected.Id);
                AssertActionsEqual(expected, actual);
            }
            
            var loadedActions = new List<AlertActionBase>();
            for(int i = 0; i < actionStore2.GetPageCount(); i++){
                loadedActions.AddRange(actionStore2.GetActions(i));
            }
            Assert.True(loadedActions.Count == 100);
        }

        void AssertActionsEqual(AlertActionBase expected, AlertActionBase actual){
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.TypeName, actual.TypeName);
            Assert.Equal(expected.Configuration, actual.Configuration);
            var logAction = expected as LogToFileAction;
            if(logAction != null){
                Assert.Equal(logAction.FileName, ((LogToFileAction)actual).FileName);
                Assert.Equal(logAction.TimestampFormat,((LogToFileAction)actual).TimestampFormat);
            }
            var consoleAction = expected as ConsoleLogAction;
            if(consoleAction != null){
                Assert.Equal(consoleAction.TimestampFormat,((ConsoleLogAction)actual).TimestampFormat);
            }
        }
    }
}