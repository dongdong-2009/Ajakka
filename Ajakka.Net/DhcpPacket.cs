using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Ajakka.Net{
    
    // Dhcp packet. See http://www.tcpipguide.com/free/t_DHCPMessageFormat.htm
    public class DhcpPacket : UdpPacket{
        const int StartIndex = 0;
        public byte BootPMessageType { get; set; }
        public HardwareType HType { get; set; }
        public byte HLen { get; set; }
        public byte Hops { get; set; }
        public UInt32 XId { get; set; }
        public ushort Secs { get; set; }
        public ushort BootPFlags { get; set; }
        public IPAddress CIAddr { get; set; }
        public IPAddress YIAddr { get; set; }
        public IPAddress SIAddr { get; set; }
        public IPAddress GIAddr { get; set; }
        public byte[] CHAddr { get; set; }
        public string ServerName { get; set; }
        public string BootFileName { get; set; }
        public UInt32 MagicCookie { get; set; }

        public bool IsActualDhcp { get; private set; }

        private readonly List<DhcpOption> options = new List<DhcpOption>();
        public List<DhcpOption> Options { get { return options; } }

        
        //Client MAC without byte padding
        public MacAddress CHAddrTruncated { get; set; }

        public DhcpPacket(byte[] bytes)
            : base(bytes){
            Bytes = bytes;
           
            Load(bytes);
        }

        private void Load(byte[] bytes){
            var index = StartIndex;
            try
            {
                //read BOOTP section
                index = LoadBootP(bytes, index);
            }
            catch(IndexOutOfRangeException)//this is not really a dhcp packet. caller is responsible to check that by verifying IsActualDhcp property
            {
                return;
            }
            if(IsActualDhcp)
            {
                index+=4;
                var factory = new DhcpOptionFactory();
                //read DHCP options
                while(index + 1 < bytes.Length && bytes[index] != 255)
                {
                    var optionLength = bytes[index + 1];
                    var option =factory.TryGetOption(bytes.SubArray(index, optionLength + 2));
                    if(option != null)
                        options.Add(option);
                    index += optionLength + 2;
                }
            }
        }

        private int LoadBootP(byte[] bytes, int index){
            if(bytes == null || bytes != null && bytes.Length < 240)
            {
                IsActualDhcp = false;
                return 0;
            }

            BootPMessageType = bytes[index++];
            HType = (HardwareType)bytes[index++];
            HLen = bytes[index++];
            Hops = bytes[index++];
            XId = ByteOps.CreateUInt32(bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3]);
            index += 4;
            Secs = ByteOps.CreateUShort(bytes[index], bytes[index + 1]);
            index += 2;
            BootPFlags = ByteOps.CreateUShort(bytes[index], bytes[index + 1]);
            index += 2;
            CIAddr = new IPAddress(new byte[] { bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3] });
            index += 4;
            YIAddr = new IPAddress(new byte[] { bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3] });
            index += 4;
            SIAddr = new IPAddress(new byte[] { bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3] });
            index += 4;
            GIAddr = new IPAddress(new byte[] { bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3] });
            index += 4;

            CHAddr = new byte[16];
            var tempAddr = new byte[6];
            for (byte i = 0; i < 16; i++)
            {
                CHAddr[i] = bytes[index + i];
                if (i < 6)
                {
                    tempAddr[i] = bytes[index + i];
                }
            }
            index += 16;
            CHAddrTruncated = new MacAddress(tempAddr);

            ServerName = ByteOps.GetAsciiString(bytes, index, 64);
            index += 64;

            BootFileName = ByteOps.GetAsciiString(bytes, index, 128);
            index += 128;

            MagicCookie = ByteOps.CreateUInt32(bytes[index], bytes[index + 1], bytes[index + 2], bytes[index + 3]);
            IsActualDhcp = MagicCookie == 0x63825363;
            return index;
        }

        public DhcpMessageType GetDhcpMessageType(){
            var messageTypes = from option in options where option is DhcpMessageTypeOption select ((DhcpMessageTypeOption)option).MessageType;
            return messageTypes.FirstOrDefault();
        }

        public string GetHostName(){
            var hostNames = from option in options where option is DhcpHostnameOption select ((DhcpHostnameOption)option).HostName;
            return hostNames.FirstOrDefault();
        }

        public MacAddress GetClientMac(){
            var macs = from option in options where option is DhcpClientIdentifierOption select ((DhcpClientIdentifierOption)option).ClientMACAddress;
            var mac = macs.FirstOrDefault();
            if(mac == null)
            {
                mac = CHAddrTruncated;
            }
            return mac;
        }

        public IPAddress GetClientIp(){
            var ips = from option in options where option is DhcpRequestedIpAddressOption select ((DhcpRequestedIpAddressOption)option).RequestedIp;
            return ips.FirstOrDefault();
        }
    }
}