using System;

namespace Ajakka.Net{
    
    public class DhcpOptionFactory{
    
        public DhcpOption GetOption(byte[] bytes){
            switch (bytes[0])
            {
                case 12:
                    return new DhcpHostnameOption(bytes);
                case 50:
                    return new DhcpRequestedIpAddressOption(bytes);
                case 53:
                    return new DhcpMessageTypeOption(bytes);
                case 61:
                    return new DhcpClientIdentifierOption(bytes);
                case 60:
                    return new DhcpVendorClassIdentifierOption(bytes);
                default:
                    return new DhcpOption(bytes);
            }    
        }

        public DhcpOption TryGetOption(byte[] bytes){
            try
            {
                return GetOption(bytes);
            }
            catch(ArgumentException)
            {
                try
                {
                    return new DhcpOption(bytes);
                }
                catch(ArgumentException)
                {
                    return null;
                }
            }
        }
    }
}