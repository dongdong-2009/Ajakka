using System.Net.NetworkInformation;

namespace Ajakka.Net{
    public class MacAddress
    {
        readonly byte[] bytes = new byte[6];

        public static readonly MacAddress None;

        public MacAddress(byte[] address){
            address.CopyTo(bytes, 0);
        }


        public override bool Equals(object comparand) {
            var mac = comparand as MacAddress;
            if (mac == null)
                return false;

            var macBytes = mac.GetAddressBytes();
            for(int i = 0; i < 6; i++)
            {
                if (macBytes[i] != bytes[i])
                    return false;
            }

            return true;
        }

        public byte[] GetAddressBytes() {
            var newArray = new byte[6];
            bytes.CopyTo(newArray,0);
            return newArray;
        }

        public override int GetHashCode() {
            return bytes.GetHashCode();
        }

        public string ToString(string separator){
            return string.Format("{0,0:X2}{6}{1,0:X2}{6}{2,0:X2}{6}{3,0:X2}{6}{4,0:X2}{6}{5,0:X2}", bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], separator);
        }

        public override string ToString() {
            return string.Format("{0,0:X2}{1,0:X2}{2,0:X2}{3,0:X2}{4,0:X2}{5,0:X2}", bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]);
        }
        
        public MacAddress(){

        }

    }
}