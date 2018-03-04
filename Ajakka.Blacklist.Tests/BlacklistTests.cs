using System;
using Xunit;
using Ajakka.Blacklist;
using System.Collections.Generic;

namespace Ajakka.Blacklist.Tests
{
    public class BlacklistTests
    {
        [Fact]
        public void ShouldAddandGetRule()
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

        [Fact]
        public void ShouldReturnPage0WithRules(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            for(int i=0;i<15;i++){
                blacklist.AddRule(new Rule{
                    Name = "r" + i,
                    Pattern = "r" + i
                });
            }
           
            var rules = blacklist.GetRules(0);
            Assert.Equal(10, rules.Length);
            for(int i = 0; i < 10; i ++){
                Assert.Equal("r"+i,rules[i].Name);
                Assert.Equal("r"+i,rules[i].Name);
            }
        }

        [Fact]
        public void ShouldReturnPage1WithRules(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            for(int i=0;i<15;i++){
                blacklist.AddRule(new Rule{
                    Name = "r" + i,
                    Pattern = "r" + i
                });
            }
           
            var rules = blacklist.GetRules(1);
            Assert.Equal(5, rules.Length);
            for(int i = 0, j=10; i < 5; i ++, j++){
                Assert.Equal("r"+j,rules[i].Name);
                Assert.Equal("r"+j,rules[i].Name);
            }
            
        }

        [Fact]
        public void ShouldReturnEmptyPageWithRules(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            for(int i=0;i<15;i++){
                blacklist.AddRule(new Rule{
                    Name = "r" + i,
                    Pattern = "r" + i
                });
            }
           
            var rules = blacklist.GetRules(2);
            Assert.True(0 == rules.Length);
            
        }

        [Fact]
        public void ShouldDeleteRule(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            var one = new Rule(){
                Name = "r1"
            };
            var two = new Rule(){
                Name = "r2"
            };
            var oneStored = blacklist.AddRule(one);
            var twoStored = blacklist.AddRule(two);
            blacklist.DeleteRule(twoStored.Id);
            Assert.Throws<InvalidOperationException>(()=>{blacklist.GetRule(twoStored.Id);});
            var oneRet = blacklist.GetRule(oneStored.Id);
            Assert.Equal(oneRet.Name, oneStored.Name);
        }

        [Fact]
        public void ShouldThrowWhenDeletingRuleThatDoesNotExist(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            var one = new Rule(){
                Name = "r1"
            };
            var two = new Rule(){
                Name = "r2"
            };
            blacklist.AddRule(one);
            var twoStored = blacklist.AddRule(two);
            
            Assert.Throws<InvalidOperationException>(()=>{blacklist.DeleteRule(Guid.NewGuid());});
        }

        [Fact]
        public void ShouldThrowExceptionWhenRuleNotFound(){
            var blacklist = BlacklistFactory.CreateBlacklist();
            var one = new Rule(){
                Name = "rule 1",
                Pattern = "123456",
            };
            one.AlertActionIds.AddRange(new[]{1,2,3,4,5});
            Assert.Throws<InvalidOperationException>(()=>{blacklist.GetRule(Guid.NewGuid());});
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
