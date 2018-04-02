using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ajakka.Alerting.Tests{
    public class AlertingStorageTests{

        [Fact]
        void BugTest_ShouldAddActionAfterLoading(){
            var actionStore = ActionStoreFactory.GetActionStore();
            actionStore.AddAction(AlertActionFactory.Create("action1", "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}"));
            actionStore.AddAction(AlertActionFactory.Create("action2", "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}"));
            actionStore.AddAction(AlertActionFactory.Create("action3", "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}"));
            ((IAlertingStorage) actionStore).Save("BugTest_ShouldAddActionAfterLoading");
            var actionStore2 = ActionStoreFactory.GetActionStore();
            ((IAlertingStorage) actionStore2).Load("BugTest_ShouldAddActionAfterLoading");
            var action2 =  AlertActionFactory.Create("action", "Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}");
            actionStore2.AddAction(action2);
        }
        
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
            
            ((IAlertingStorage)actionStore).Save("ShouldSaveAndLoadActions");
            var actionStore2 = ActionStoreFactory.GetActionStore();
            var action2 = AlertActionFactory.Create("action e", "Ajakka.Alerting.LogToFileAction","{TimestampFormat:\"MM\",FileName:\"log.log\"}");
            actionStore2.AddAction(action2);

            ((IAlertingStorage)actionStore2).Load("ShouldSaveAndLoadActions");
        

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

        [Fact]
        void ShouldSaveAndLoadRuleLinks(){
            var store = ActionStoreFactory.GetActionStore();
            var ruleId = Guid.NewGuid();
            var ruleId2 = Guid.NewGuid();
            var expected1 = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
            var returned1 = store.AddAction(expected1);
            var expected2 = (LogToFileAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.LogToFileAction","{FileName:\"log.log\"}");
            var returned2 = store.AddAction(expected1);
            var expected3 = (HttpRequestAlertAction)AlertActionFactory.Create("send http request","Ajakka.Alerting.HttpRequestAlertAction","{Url:\"http://google.com\"}");
            var returned3 = store.AddAction(expected3);

            store.LinkRuleToAction(ruleId,returned1.Id);
            store.LinkRuleToAction(ruleId,returned2.Id);
            store.LinkRuleToAction(ruleId2, returned3.Id);
            ((IAlertingStorage)store).Save("ShouldSaveAndLoadRuleLinks");
            var store2 = ActionStoreFactory.GetActionStore();
            ((IAlertingStorage)store2).Load("ShouldSaveAndLoadRuleLinks");
        
            var actions = store2.GetLinkedActions(ruleId);
            Assert.True(actions.Length == 2);
            var action1 = actions.First((a)=>{return a.Id == returned1.Id;});
            AssertActionsEqual(returned1, action1);
            var action2 = actions.First((a)=>{return a.Id == returned2.Id;});
            AssertActionsEqual(returned2, action2);

            var action3 = store2.GetLinkedActions(ruleId2).First((a)=>{return a.Id == returned3.Id;});
            AssertActionsEqual(returned3, action3);
            
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