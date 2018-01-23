using System;
using System.Text;

namespace Ajakka.Net{
    public static class ByteOps
    {
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
    }
}