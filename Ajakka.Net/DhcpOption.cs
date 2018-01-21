using System;

namespace Ajakka.Net{
    public class DhcpOption{
        public byte Type { get; set; }
        public byte Length { get; set; }
        public byte[] Value { get; set; }

        public DhcpOption(){

        }

        public DhcpOption(byte[] bytes){
            if (bytes.Length < 3)
                throw new ArgumentException("The length of the array needs to be at least 3");
            Type = bytes[0];
            Length = bytes[1];
            if (Length != bytes.Length - 2)
                throw new ArgumentException("Specified option length is not correct");
            Value = new byte[Length];
            for(int i = 2; i <bytes.Length; i++)
            {
                Value[i - 2] = bytes[i];
            }
        }

        public virtual string GetStringValue(){
            return ByteOps.GetAsciiString(Value, 0, Value.Length);
        }

        public override string ToString(){
            return string.Format("DhcpOption type {0}: {1}", Type, GetStringValue());
        }
    }
}