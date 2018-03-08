using System;
using Xunit;
using Ajakka.Blacklist;

namespace Ajakka.Blacklist.Tests
{
    public class RuleTests
    {
        [Fact]
        public void ShouldCloneRule(){
            var expected = new Rule(){
                Name = "rule 1",
                Pattern = "123456",
                Id = Guid.NewGuid(),
            };
            expected.AlertActionIds.AddRange(new[]{1,2,3,4});
            var actual = (Rule)expected.Clone();
            Assert.Equal(expected.Id,actual.Id);
            Assert.Equal(expected.AlertActionIds,actual.AlertActionIds);
            Assert.Equal(expected.Name,actual.Name);
            Assert.Equal(expected.Pattern,actual.Pattern);
        }

        [Fact]
        public void ShouldMatch(){
            var rule = new Rule(){
                Name = "r1",
                Pattern = "123456",
                Id = Guid.NewGuid()
            };
            Assert.True(rule.IsMatch("12345678"));
            Assert.True(rule.IsMatch("812312345678"));
        }

        [Fact]
        public void ShouldNotMatch(){
            var rule = new Rule(){
                Name = "r1",
                Pattern = "123456",
                Id = Guid.NewGuid()
            };
            Assert.False(rule.IsMatch("123"));
            Assert.False(rule.IsMatch("5678987654"));
        }
    }
}