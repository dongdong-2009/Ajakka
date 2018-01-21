namespace Ajakka.Net{
    public class DhcpVendorClassIdentifierOption:DhcpOption
    {
        public string VendorClassId { get { return GetStringValue(); } }
        
        public DhcpVendorClassIdentifierOption() { }

        public DhcpVendorClassIdentifierOption(byte[] bytes):base(bytes){

        }

        public override string ToString(){
            return string.Format("Vendor Class Identifier: {0}", VendorClassId);
        }
    }
}