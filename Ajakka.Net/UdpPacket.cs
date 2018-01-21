using System;
using System.Net;

namespace Ajakka.Net
{
    public class UdpPacket{
        static ulong CurrentId = 0;

        public ulong Id { get; private set; }
        public byte[] Bytes { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public UdpPacket(byte[] bytes){
            Id = CurrentId++;
            Bytes = bytes;
            TimeStamp = DateTime.UtcNow;
        }

    }
}
