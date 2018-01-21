using System;
using System.Text;

namespace Ajakka.Net{
    public static class ByteOps
    {
        public static ushort CreateUShort(byte high, byte low)
        {
            return (ushort)((high << 8) + low);
        }

        
        // Creates Int32 from 4 bytes
        public static UInt32 CreateUInt32(byte d, byte c, byte b, byte a)
        {
            
            return (UInt32)((d << 24) + (c << 16) + (b << 8) + a);
        }

        public static string GetAsciiString(byte[] bytes, int index, int count)
        {
            return Encoding.ASCII.GetString(bytes, index, count).TrimEnd(new []{'\0'});
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static bool IsEmpty(this MacAddress address)
        {
            foreach(var b in address.GetAddressBytes())
            {
                if (b != 0)
                    return false;
            }
            return true;
        }

        public static byte[] GetBytes(uint v)
        {
            byte[] result = new byte[]
            {
                (byte)(v & 0xff),
                (byte)((v & 0xff00) / 0x100),
                (byte)((v & 0xff0000) / 0x10000),
                (byte)((v & 0xff000000) / 0x1000000)
            };
            return result;
        }

      
    }
}