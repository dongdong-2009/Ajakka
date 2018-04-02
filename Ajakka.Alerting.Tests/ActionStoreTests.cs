using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Ajakka.Alerting.Tests{
    public class ActionStoreTests{
        [Fact]
        public void ShouldAddAndGetAlertActions(){
            var store = ActionStoreFactory.GetActionStore();
            ConsoleLogAction expected1 = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
            ConsoleLogAction expected2 = (ConsoleLogAction)AlertActionFactory.Create("log to console 2","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}");
           
            var returned1 = store.AddAction(expected1);
            var returned2 = store.AddAction(expected2);
           
            var actual1 = store.GetAction(returned1.Id);
            var actual2 = store.GetAction(returned2.Id);

            AssertActionsEqual(expected1, actual1, false);
            AssertActionsEqual(expected2, actual2, false);
            AssertActionsEqual(returned1, actual1);
            AssertActionsEqual(returned2, actual2);
        }

        [Fact]
        public void ShouldReturnPageWithActions(){
            var store = ActionStoreFactory.GetActionStore();
            for(int i = 0; i < 15; i++){
                var action = (ConsoleLogAction)AlertActionFactory.Create("log to console " + i,"Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
                store.AddAction(action);
            }
            var firstPage = store.GetActions(0);
            Assert.Equal(10, firstPage.Length);
            for(int i = 0; i < 10; i++){
                Assert.Equal("log to console "+ i, firstPage[i].Name);
            }
        }

        [Fact]
        public void ShouldReturnSecondPageWithActions(){
            var store = ActionStoreFactory.GetActionStore();
            for(int i = 0; i < 15; i++){
                var action = (ConsoleLogAction)AlertActionFactory.Create("log to console " + i,"Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
                store.AddAction(action);
            }
            var firstPage = store.GetActions(1);
            Assert.Equal(5, firstPage.Length);
            for(int i = 0,j=10; i < 5; i++,j++){
                Assert.Equal("log to console "+ j, firstPage[i].Name);
            }
        }

        [Fact]
        public void ShouldReturnEmptyPageWithActions(){
            var store = ActionStoreFactory.GetActionStore();
            for(int i = 0; i < 15; i++){
                var action = (ConsoleLogAction)AlertActionFactory.Create("log to console " + i,"Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
                store.AddAction(action);
            }
            var firstPage = store.GetActions(3);
            Assert.True(0 == firstPage.Length);
        }

        [Fact]
        public void ShouldPageCount(){
            var store = ActionStoreFactory.GetActionStore();
            for(int i = 0; i < 15; i++){
                var action = (ConsoleLogAction)AlertActionFactory.Create("log to console " + i,"Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
                store.AddAction(action);
            }
            var pageCount = store.GetPageCount();
            Assert.Equal(2, pageCount);
        }
        
        [Fact]
        public void ShouldDeleteAction(){
            var store = ActionStoreFactory.GetActionStore();
            ConsoleLogAction expected1 = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
            ConsoleLogAction expected2 = (ConsoleLogAction)AlertActionFactory.Create("log to console 2","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"MM\"}");
            var returned1 = store.AddAction(expected1);
            var returned2 = store.AddAction(expected2);

            store.DeleteAction(returned1.Id);           
            var actual2 = store.GetAction(returned2.Id);

            AssertActionsEqual(expected2, actual2, false);
            AssertActionsEqual(returned2, actual2);
            Assert.Throws<KeyNotFoundException>(()=>{store.GetAction(returned1.Id);});
        }

        [Fact]
        public void ShouldUpdateAction(){
            var store = ActionStoreFactory.GetActionStore();
            ConsoleLogAction expected1 = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
            var returned = store.AddAction(expected1);

            expected1.Name = "changed";
            expected1.TimestampFormat = "MM";
            expected1.Configuration = "{TimestampFormat:\"MM\"}";

            store.UpdateAction(returned.Id, expected1);

            var actual = store.GetAction(returned.Id);
            AssertActionsEqual(expected1, actual, false);
        }

        [Fact]
        public void ShouldLinkActionToBlacklistRule(){
            var store = ActionStoreFactory.GetActionStore();
            var ruleId = Guid.NewGuid();
            ConsoleLogAction expected1 = (ConsoleLogAction)AlertActionFactory.Create("log to console","Ajakka.Alerting.ConsoleLogAction","{TimestampFormat:\"G\"}");
            var returned = store.AddAction(expected1);

            store.LinkRuleToAction(ruleId,returned.Id);
            var actions = store.GetLinkedActions(ruleId);
            Assert.True(actions.Length == 1);
            AssertActionsEqual(returned, actions[0], true);
        }

        [Fact]
        public void ShouldLinkActionsToBlacklistRule(){
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

            var actions = store.GetLinkedActions(ruleId);
            Assert.True(actions.Length == 2);
            var action1 = actions.First((a)=>{return a.Id == returned1.Id;});
            AssertActionsEqual(returned1, action1, true);
            var action2 = actions.First((a)=>{return a.Id == returned2.Id;});
            AssertActionsEqual(returned2, action2, true);

            var action3 = store.GetLinkedActions(ruleId2).First((a)=>{return a.Id == returned3.Id;});
            AssertActionsEqual(returned3, action3, true);
        }

        private void AssertActionsEqual(AlertActionBase expected, AlertActionBase actual, bool compareIds = true){
            if(compareIds){
                Assert.Equal(expected.Id, actual.Id);
            }
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Configuration, actual.Configuration);
        }
    }
}