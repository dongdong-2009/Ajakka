namespace Ajakka.Net{
    
    public class DhcpHostnameOption:DhcpOption{
        public string HostName { get { return GetStringValue(); } }
        
        public DhcpHostnameOption(){

        }

        public DhcpHostnameOption(byte[] bytes):base(bytes){

        }

        public override string ToString(){
            return string.Format("Hostname: {0}", HostName);
        }
    }
}