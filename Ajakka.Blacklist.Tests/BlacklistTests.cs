using System;
using Xunit;
using Ajakka.Blacklist;

namespace Ajakka.Blacklist.Tests
{
    public class BlacklistTests
    {
        [Fact]
        public void ShouldAddRule()
        {
            var blacklist = BlacklistFactory.CreateBlacklist();
            var expected = new Rule(){
                Name = "rule 1",
                Pattern = "123456",
            };
            expected.AlertActionIds.AddRange(new[]{1,2,3,4,5});
            var returnedRule = blacklist.AddRule(expected);

            var actual = blacklist.GetRule(returnedRule.Id);
            AssertRulesEqual(expected, actual, false);
            AssertRulesEqual(expected, returnedRule, false);
            AssertRulesEqual(returnedRule, actual, false);
        }

        [Fact]
        public void ShouldAddRulesIgnoreId()
        {
            var blacklist = BlacklistFactory.CreateBlacklist();
            var expected1 = new Rule(){
                Name = "rule 1",
                Pattern = "123456",
                Id = Guid.NewGuid(),
            };
            expected1.AlertActionIds.AddRange(new[]{1,2,3,4,5});
            var expected2 = (Rule)expected1.Clone();
            Assert.Equal(expected1.Id, expected2.Id);

            var returnedRule1 = blacklist.AddRule(expected1);
            var returnedRule2 = blacklist.AddRule(expected2);

            var actual1 = blacklist.GetRule(returnedRule1.Id);
            AssertRulesEqual(expected1, actual1, false);
            AssertRulesEqual(expected1, returnedRule1, false);
            AssertRulesEqual(returnedRule1, actual1, false);

            var actual2 = blacklist.GetRule(returnedRule2.Id);
            AssertRulesEqual(expected2, actual2, false);
            AssertRulesEqual(expected2, returnedRule2, false);
            AssertRulesEqual(returnedRule2, actual2, false);
        }

        [Fact]
        public void ShouldNotAddRuleWithoutName(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            var expected = new Rule(){
                Name = "",
                Pattern = "123456",
            };
           Assert.Throws<ArgumentException>(()=>{
                blacklist.AddRule(expected);
           });
        }

        void AssertRulesEqual(Rule expected, Rule actual, bool compareId){
            if(compareId){
                Assert.Equal(expected.Id, actual.Id);
            }
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Pattern, actual.Pattern);
            Assert.Equal(expected.AlertActionIds, actual.AlertActionIds);
        }
    }
}
