using System;
using Xunit;
using Ajakka.Net;

namespace Ajakka.Net.UnitTest
{
    public class Tests
    {

        [Fact]
        public void CanGetStringFromBytes()
        {
            var bytes = new byte[] { 65, 66, 67, 0, 0, 0 };
            var expected1 = "ABC";
            var expected2 = "BC";
            var actual1 = ByteOps.GetAsciiString(bytes, 0, 6);
            var actual2 = ByteOps.GetAsciiString(bytes, 1, 5);
            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
        }




    }
}
