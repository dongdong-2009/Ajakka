using System;
using Xunit;
using Ajakka.Net;
using System.Net;
using System.Collections.Generic;

namespace Ajakka.Net.UnitTest{
   
    public class MacAddressTest
    {
        [Fact]
        public void ShouldCreateMacAddressFromBytes()
        {
            var mac = new MacAddress(new byte[] {0,1,2,3,4,5});
            var bytes = mac.GetAddressBytes();
            Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5 }, bytes);
        }


        [Fact]
        public void ShouldParseMacAddressRaw()
        {
            var mac = MacAddress.Parse("3A6EA5112233");
            Assert.Equal(new byte[] { 0x3a, 0x6e, 0xa5, 0x11, 0x22, 0x33 }, mac.GetAddressBytes());
        }

        [Fact]
        public void ShouldParseMacAddressWithDash()
        {
            var mac = MacAddress.Parse("3a-6e-a5-11-22-33");
            Assert.Equal(new byte[] { 0x3a, 0x6e, 0xa5, 0x11, 0x22, 0x33 }, mac.GetAddressBytes());
        }

        [Fact]
        public void ShouldCompareEqual()
        {
            var mac1 = new MacAddress(new byte[] { 0, 1, 2, 3, 4, 5 });
            var mac2 = new MacAddress(new byte[] { 0, 1, 2, 3, 4, 5 });
            Assert.True(mac1.Equals(mac2));
        }

        [Fact]
        public void ShouldCompareNotEqual()
        {
            var mac1 = new MacAddress(new byte[] { 0, 1, 2, 3, 4, 5 });
            var mac2 = new MacAddress(new byte[] { 0, 1, 2, 2, 4, 5 });
            Assert.False(mac1.Equals(mac2));
        }

        [Fact]
        public void ShouldConvertToString()
        {
            var mac1 = new MacAddress(new byte[] { 0, 1, 2, 3, 4, 15 });
            Assert.Equal("00010203040F", mac1.ToString());
        }

        [Fact]
        public void ShouldThrowArgumentNullException(){
            Assert.Throws<ArgumentNullException>(()=>{
                MacAddress.Parse("");
            });
        }

        [Fact]
        public void ShouldThrowArgumentException(){
            Assert.Throws<ArgumentException>(()=>{
                MacAddress.Parse("1234567");
            });
        }
    }
}