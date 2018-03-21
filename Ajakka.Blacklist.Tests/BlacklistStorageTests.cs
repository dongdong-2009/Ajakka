using System.Collections.Generic;
using Xunit;

namespace Ajakka.Blacklist.Tests{
    public class BlacklistStorageTests{

        [Fact]
        void ShouldSaveAndLoadRules(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            List<Rule> rules = new List<Rule>();
            for(int i =0; i < 100; i++){
                var rule = new Rule("r" + i, i.ToString(),new int[]{i});
                blacklist.AddRule(rule);
            }
            
            ((IBlacklistStorage)blacklist).Save("blacklist_test");
            var blacklist2 = BlacklistFactory.CreateBlacklist();
            blacklist2.AddRule(new Rule(){Name="extra"});
            ((IBlacklistStorage)blacklist2).Load("blacklist_test");
        

            foreach(var expected in rules){
                var actual = blacklist2.GetRule(expected.Id);
                AssertRulesEqual(expected, actual);
            }
            
            var loadedRules = new List<Rule>();
            for(int i = 0; i < blacklist2.GetRulePageCount(); i++){
                loadedRules.AddRange(blacklist2.GetRules(i));
            }
            Assert.True(loadedRules.Count == 100);
        }

        void AssertRulesEqual(Rule expected, Rule actual){
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Pattern, actual.Pattern);
            Assert.Equal(expected.AlertActionIds, actual.AlertActionIds);
        }
    }
}