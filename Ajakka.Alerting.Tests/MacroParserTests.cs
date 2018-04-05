using Newtonsoft.Json;
using Xunit;

namespace Ajakka.Alerting.Tests{
    public class MacroParserTests{
        dynamic inputDevice = new {
            Mac = "001122334455",
            Ip = "192.168.1.1",
            Name = "server"
        };

        [Fact]
        void ShouldTranslateMac(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?mac={Mac}",inputDevice);
            Assert.Equal("http://example.com?mac=001122334455",parsed);
        }

        [Fact]
        void ShouldTranslateMacAndIp(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?mac={Mac}&ip={Ip}",inputDevice);
            Assert.Equal("http://example.com?mac=001122334455&ip=192.168.1.1",parsed);
        }

        [Fact]
        void ShouldTranslateMacAndIpAndName(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?mac={Mac}&ip={Ip}&name={Name}",inputDevice);
            Assert.Equal("http://example.com?mac=001122334455&ip=192.168.1.1&name=server",parsed);
        }


        [Fact]
        void ShouldNotTranslateNonMacroStrings(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={Mac}&Ip={Ip}",inputDevice);
            Assert.Equal("http://example.com?Mac=001122334455&Ip=192.168.1.1",parsed);
        }

        [Fact]
        void ShouldIgnoreUnfinishedMacros(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={Mac}&Ip={Ip",inputDevice);
            Assert.Equal("http://example.com?Mac=001122334455&Ip={Ip",parsed);
        }

        [Fact]
        void ShouldIgnoreUnfinishedMacros2(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={Mac&Ip={Ip}",inputDevice);
            Assert.Equal("http://example.com?Mac={Mac&Ip=192.168.1.1",parsed);
        }

        [Fact]
        void ShouldIgnoreUnfinishedMacros3(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={Mac&Ip=1234",inputDevice);
            Assert.Equal("http://example.com?Mac={Mac&Ip=1234",parsed);
        }
        [Fact]
        void ShouldIgnoreUnfinishedMacros4(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac=}Mac&Ip=1234",inputDevice);
            Assert.Equal("http://example.com?Mac=}Mac&Ip=1234",parsed);
        }
        [Fact]
        void ShouldIgnoreEmptyMacros(){
           var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={}1",inputDevice);
            Assert.Equal("http://example.com?Mac={}1",parsed);
        }

        
        [Fact]
        void ShouldNotParseMissingProperties(){
            var parser = new MacroParser();
            
            var parsed = parser.Parse("http://example.com?Mac={aaa}1",inputDevice);
            Assert.Equal("http://example.com?Mac={aaa}1",parsed);
        }
    }
}