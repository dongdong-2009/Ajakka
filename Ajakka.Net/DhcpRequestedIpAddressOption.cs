using System.Net;

namespace Ajakka.Net{
    public class DhcpRequestedIpAddressOption:DhcpOption{
        public IPAddress RequestedIp { get; set; }
         public DhcpRequestedIpAddressOption(){
            
        }

         public DhcpRequestedIpAddressOption(byte[] bytes)
            : base(bytes){
            if (bytes[1] != 4 || bytes.Length != 6)
                return;

             RequestedIp = new IPAddress(new byte[]{bytes[2],bytes[3], bytes[4], bytes[5]});
        }

        public override string GetStringValue(){
            return string.Format("{0}", RequestedIp);
        }

        public override string ToString(){
            return string.Format("Requested IP: {0}", RequestedIp);
        }
    }
}