using System;
using Xunit;
using Ajakka.Net;
using System.Net;

namespace Ajakka.Net.UnitTest{

    public class DhcpOptionFactoryTest{
        
        [Fact]
        public void CreatesGenericDhcpOption()
        {
            byte[] option = new byte[] { 255, 2, 65, 66 };
            var factory = new DhcpOptionFactory();
            var result = factory.GetOption(option);
            Assert.Equal(255, result.Type);
            Assert.Equal(2, result.Length);
            Assert.Equal(new byte[]{65,66},result.Value);
            Assert.Equal("AB", result.GetStringValue());
        }

        [Fact]
        public void CreatesDhcpHostnameOption()
        {
            byte[] option = new byte[] { 12, 6, 65, 66, 67, 68, 69, 70 };
            var factory = new DhcpOptionFactory();
            var result = factory.GetOption(option) as DhcpHostnameOption;
            Assert.NotNull(result); //verifies the returned type is correct.
            Assert.Equal(12, result.Type);
            Assert.Equal(6, result.Length);
            Assert.Equal("ABCDEF", result.HostName);
        }

        [Fact]
        public void CreatesVendorClassIdOption()
        {
            byte[] option = new byte[] { 60, 6, 65, 66, 67, 68, 69, 70 };
            var factory = new DhcpOptionFactory();
            var result = factory.GetOption(option) as DhcpVendorClassIdentifierOption;
            Assert.NotNull(result); //verifies the returned type is correct.
            Assert.Equal(60, result.Type);
            Assert.Equal(6, result.Length);
            Assert.Equal("ABCDEF", result.VendorClassId);
        }

        [Fact]
        public void CreatesMessagesTypeOption()
        {
            byte[] option = new byte[] { 53, 1, 3};
            var factory = new DhcpOptionFactory();
            var result = factory.GetOption(option) as DhcpMessageTypeOption;
            Assert.NotNull(result); //verifies the returned type is correct.
            Assert.Equal(53, result.Type);
            Assert.Equal(1, result.Length);
            Assert.Equal(DhcpMessageType.Request, result.MessageType);
        }

       [Fact]
        public void CreatesDhcpClientIdOption()
        {
            byte[] option = new byte[] { 61, 7, 1, 0x8c, 0x70, 0x5a, 1,2,3 };
            var factory = new DhcpOptionFactory();
            var result = factory.GetOption(option) as DhcpClientIdentifierOption;
            Assert.NotNull(result); //verifies the returned type is correct.
            Assert.Equal(61, result.Type);
            Assert.Equal(7, result.Length);
            Assert.Equal(ArpHardwareType.Ethernet, result.HardwareType);
            Assert.Equal(new byte[]{0x8c,0x70,0x5a,1,2,3}, result.ClientMACAddress.GetAddressBytes());
        }

        
        [Fact]
        public void ShouldThrowWhenMessageTypeOptionLengthIncorrect()
        {
            byte[] option = new byte[] { 53, 2, 3, 5 };
            var factory = new DhcpOptionFactory();
            Assert.Throws<ArgumentException>(()=>{
                var result = factory.GetOption(option) as DhcpMessageTypeOption;
            });
        }

        [Fact]
        public void ShouldThrowWhenOptionLengthIncorrect1()
        {
            byte[] option = new byte[] { 255, 3, 65, 66 };
            var factory = new DhcpOptionFactory();
            Assert.Throws<ArgumentException>(()=>{
                var result = factory.GetOption(option);
            });
        }

        [Fact]
        public void ShouldThrowWhenOptionLengthIncorrect2()
        {
            byte[] option = new byte[] { 255, 1, 65, 66 };
            var factory = new DhcpOptionFactory();
            Assert.Throws<ArgumentException>(()=>{
                var result = factory.GetOption(option);
            });
        }

        [Fact]
        public void TryGetShouldReturnNullWhenDataNotValid()
        {
            byte[] option = new byte[] { 255, 3, 65, 66 };
            var factory = new DhcpOptionFactory();
            var result = factory.TryGetOption(option);
            Assert.Null(result);
        }

       [Fact]
        public void TryGetReturnsValidOption()
        {
            byte[] option = new byte[] { 53, 2, 65, 66};
            var factory = new DhcpOptionFactory();
            var result = factory.TryGetOption(option);
            Assert.NotNull(result);
            Assert.Equal("AB", result.GetStringValue());
        }

       [Fact]
        public void TryGetReturnsValidRequestedIpOption()
        {
            byte[] option = new byte[] { 0x32, 0x4, 0xc0, 0xa8, 0x01, 0x07 };
            var factory = new DhcpOptionFactory();
            var result = factory.TryGetOption(option) as DhcpRequestedIpAddressOption;
            Assert.NotNull(result);
            Assert.Equal(IPAddress.Parse("192.168.1.7"), result.RequestedIp);
        }        
    }
}