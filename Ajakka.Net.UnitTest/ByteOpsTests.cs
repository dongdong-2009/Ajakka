﻿using System;
using Xunit;
using Ajakka.Net;

namespace Ajakka.Net.UnitTest
{
    public class Tests
    {

        [Fact]
        public void CanCreateUShortFromBytes()
        {
            var a = ByteOps.CreateUShort(0x1, 0x13);
            Assert.Equal(275, a);
        }
        
        [Fact]
        public void CanCreateUint32FromBytes()
        {
            var a = ByteOps.CreateUInt32(0x1, 0x13,0xff,0x10);
            Assert.Equal((UInt32)0x113ff10, a);

            var b = ByteOps.CreateUInt32(0xe4, 0xaa, 0xe1, 0xef);
            Assert.Equal((UInt32)0xe4aae1ef, b);
        }

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
