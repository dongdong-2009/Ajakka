using System;
using Xunit;
using Ajakka.Net;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Ajakka.Net.UnitTest{
    public class DhcpPacketTest
    {
        static string pathToBinFiles = string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar);
        
        [Fact]
        public void ShouldParseDhcpRequest()
        {
            using (var stream = new FileStream(pathToBinFiles + "dhcprequest.bin", FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                var bytes = reader.ReadBytes((int)stream.Length);
                DhcpPacket packet = new DhcpPacket(bytes);
                Assert.Equal(1, packet.BootPMessageType);
                Assert.Equal(HardwareType.Ethernet, packet.HType);
                Assert.Equal(6, packet.HLen);
                Assert.Equal(0, packet.Hops);
                Assert.Equal((UInt32)0x582d2660, packet.XId);
                Assert.Equal(0, packet.Secs);
                Assert.Equal(0x8000, packet.BootPFlags);
                Assert.Equal(IPAddress.Any, packet.CIAddr);
                Assert.Equal(IPAddress.Any, packet.YIAddr);
                Assert.Equal(IPAddress.Any, packet.GIAddr);
                Assert.Equal(IPAddress.Any, packet.SIAddr);

                var physAddressExpected = new byte[] { 0xf4, 0xf5, 0xa5, 0x83, 0x2d, 0x16 };
                var physAddressActual = packet.CHAddrTruncated;
                
                Assert.Equal(new List<byte>(physAddressExpected), new List<byte>(physAddressActual.GetAddressBytes()));
                Assert.Equal(string.Empty, packet.ServerName);
                Assert.Equal(string.Empty, packet.BootFileName);
                
                Assert.True(packet.IsActualDhcp);

                Assert.Equal(8, packet.Options.Count);
                Assert.Equal(DhcpMessageType.Request, packet.GetDhcpMessageType());
            }
        }

        [Fact]
        public void ShouldParseDhcpDiscover()
        {
            using (var stream = new FileStream(pathToBinFiles + "dhcpdiscover.bin", FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                var bytes = reader.ReadBytes((int)stream.Length);
                DhcpPacket packet = new DhcpPacket(bytes);
                Assert.Equal(1, packet.BootPMessageType);
                Assert.Equal(HardwareType.Ethernet, packet.HType);
                Assert.Equal(6, packet.HLen);
                Assert.Equal(0, packet.Hops);
                Assert.Equal(0x9d2b3df6, packet.XId);
                Assert.Equal(0, packet.Secs);
                Assert.Equal(0x8000, packet.BootPFlags);
                Assert.Equal(IPAddress.Any, packet.CIAddr);
                Assert.Equal(IPAddress.Any, packet.YIAddr);
                Assert.Equal(IPAddress.Any, packet.GIAddr);
                Assert.Equal(IPAddress.Any, packet.SIAddr);

                var physAddressExpected = new byte[] { 0xf4, 0xf5, 0xa5, 0x83, 0x2d, 0x16 };
                var physAddressActual = packet.CHAddrTruncated;

                Assert.Equal(new List<byte>(physAddressExpected), new List<byte>(physAddressActual.GetAddressBytes()));
                Assert.Equal(string.Empty, packet.ServerName);
                Assert.Equal(string.Empty, packet.BootFileName);

                Assert.True(packet.IsActualDhcp);

                Assert.Equal(5, packet.Options.Count);
                Assert.Equal(DhcpMessageType.Discover, packet.GetDhcpMessageType());
            }
        }

        [Fact]
        public void ShouldLoadEmptyPacket()
        {
            var packet = new DhcpPacket(new byte[300]);
            Assert.Equal(DhcpMessageType.Unknown, packet.GetDhcpMessageType());
        }
    }
}