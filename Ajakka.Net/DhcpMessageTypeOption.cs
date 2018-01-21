using System;

namespace Ajakka.Net{
    public class DhcpMessageTypeOption : DhcpOption{
        public string HostName { get { return GetStringValue(); } }
        public DhcpMessageType MessageType { get; set; }

        public DhcpMessageTypeOption(){

        }

        public DhcpMessageTypeOption(byte[] bytes)
            : base(bytes){
            if (Length != 1)
                throw new ArgumentException("Option length is not valid for this option");

            MessageType = (DhcpMessageType)Value[0];
        }

        public override string GetStringValue(){
            return MessageType.ToString();
        }

        public override string ToString(){
            return string.Format("MessageType: {0}", MessageType);
        }
    }

    
}