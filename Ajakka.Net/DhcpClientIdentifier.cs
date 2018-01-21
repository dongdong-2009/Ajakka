namespace Ajakka.Net{
    public class DhcpClientIdentifierOption : DhcpOption{
            public ArpHardwareType HardwareType { get; set; }
            public MacAddress ClientMACAddress { get; set; }

            public DhcpClientIdentifierOption(){
                
            }

            public DhcpClientIdentifierOption(byte[] bytes)
                : base(bytes){
                HardwareType = (ArpHardwareType)Value[0];
                if (HardwareType == ArpHardwareType.Ethernet && Length == 7) //ethernet
                {
                    byte[] physAddress = new byte[6];
                    for (int i = 0; i < 6; i++)
                    {
                        physAddress[i] = Value[i + 1];
                    }
                    ClientMACAddress = new MacAddress(physAddress);
                }
                //else ignore this option
            }

            public override string GetStringValue(){
                return string.Format("{0}-{1}", HardwareType, ClientMACAddress);
            }

            public override string ToString(){
                return string.Format("Client Identifier: {0}-{1}", HardwareType, ClientMACAddress);
            }
        }

        
}