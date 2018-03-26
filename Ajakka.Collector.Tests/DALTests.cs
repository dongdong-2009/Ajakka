using System;
using Xunit;
using Ajakka.Collector;
using Xunit.Abstractions;
using MySql.Data.MySqlClient;
using System.Data;

namespace Ajakka.Collector.Tests
{
    public class DALTests
    {
        private readonly ITestOutputHelper output;
        private readonly string testDbConnectionString;

        public DALTests(ITestOutputHelper output)
        {
            this.output = output;
            testDbConnectionString = Environment.GetEnvironmentVariable("AjakkaTestConnection");
            if(string.IsNullOrEmpty(testDbConnectionString)){
              //  testDbConnectionString = "server=35.195.71.90;uid=root;Password=jncnr5kinzo0HDwp;database=ajakka";
                output.WriteLine("AjakkaTestConnection environment variable is not set.");
            }
        }

        private void ClearEndpointsTable()
        {
            if(string.IsNullOrEmpty(testDbConnectionString)){
                output.WriteLine("AjakkaTestConnection environment variable is not set.");
                throw new InvalidOperationException();
            }
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(testDbConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "delete from endpoint_latest";
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void ShouldStoreEndpoint()
        {
            ClearEndpointsTable();

            ICollectorDAL dal = new DAL(testDbConnectionString);
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("0123456","192.168.1.1","mike",expectedTimestamp, "sensor1");
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(testDbConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM endpoint_latest";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;
                        var mac = record[0].ToString();
                        var ip = record[1].ToString();
                        var hostname = record[2].ToString();
                        var timestamp = (DateTime)record[3];
                        var detectedBy = record[4].ToString();
                        Assert.Equal("0123456", mac);
                        Assert.Equal("192.168.1.1", ip);
                        Assert.Equal("mike", hostname);
                        Assert.True((expectedTimestamp - timestamp) < TimeSpan.FromSeconds(1));
                        Assert.Equal("sensor1", detectedBy);
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateEndpoint()
        {
            ClearEndpointsTable();

            ICollectorDAL dal = new DAL(testDbConnectionString);
            dal.StoreDhcpEndpoint("0123456","192.168.1.1","mike",DateTime.Now.AddDays(-1),"sensor1");
           
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("0123456","192.168.1.13","mike2",expectedTimestamp,"sensor2");
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(testDbConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM endpoint_latest";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;
                        var mac = record[0].ToString();
                        var ip = record[1].ToString();
                        var hostname = record[2].ToString();
                        var timestamp = (DateTime)record[3];
                        var detectedBy = record[4].ToString();
                        Assert.Equal("0123456", mac);
                        Assert.Equal("192.168.1.13", ip);
                        Assert.Equal("mike2", hostname);
                        Assert.True((expectedTimestamp - timestamp) < TimeSpan.FromSeconds(1));
                        Assert.Equal("sensor2", detectedBy);
                    }
                }
            }
        }
        
        [Fact]
        public void ShouldReturnFirstPageWithEndpoints(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("012345678901","192.168.1.0","mike0",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("112345678901","192.168.1.1","mike1",expectedTimestamp.Subtract(TimeSpan.FromSeconds(1)), "sensor1");
            dal.StoreDhcpEndpoint("212345678901","192.168.1.2","mike2",expectedTimestamp.Subtract(TimeSpan.FromSeconds(2)), "sensor1");
            dal.StoreDhcpEndpoint("312345678901","192.168.1.3","mike3",expectedTimestamp.Subtract(TimeSpan.FromSeconds(3)), "sensor1");
            dal.StoreDhcpEndpoint("412345678901","192.168.1.4","mike4",expectedTimestamp.Subtract(TimeSpan.FromSeconds(4)), "sensor1");
            dal.StoreDhcpEndpoint("512345678901","192.168.1.5","mike5",expectedTimestamp.Subtract(TimeSpan.FromSeconds(5)), "sensor1");
            dal.StoreDhcpEndpoint("612345678901","192.168.1.6","mike6",expectedTimestamp.Subtract(TimeSpan.FromSeconds(6)), "sensor1");
            dal.StoreDhcpEndpoint("712345678901","192.168.1.7","mike7",expectedTimestamp.Subtract(TimeSpan.FromSeconds(7)), "sensor1");
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            var endpoints = dal.GetEndpoints(0,5);
            for(int i = 0; i < 5; i ++){
                Assert.Equal(i + "12345678901",endpoints[i].DeviceMacAddress);
                Assert.Equal("192.168.1."+i,endpoints[i].DeviceIpAddress);
                Assert.Equal("mike"+i,endpoints[i].DeviceName);
                Assert.True(Math.Abs((expectedTimestamp - endpoints[i].TimeStamp).TotalSeconds) < i+0.9);
                Assert.Equal("sensor1", endpoints[i].DetectedBy);
            }

            Assert.Equal(5, endpoints.Length);
        }

        [Fact]
        public void ShouldReturnSecondPageWithEndpoints(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("012345678901","192.168.1.0","mike0",expectedTimestamp,"sensor1");
            dal.StoreDhcpEndpoint("112345678901","192.168.1.1","mike1",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("212345678901","192.168.1.2","mike2",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("312345678901","192.168.1.3","mike3",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("412345678901","192.168.1.4","mike4",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("512345678901","192.168.1.5","mike5",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("612345678901","192.168.1.6","mike6",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("712345678901","192.168.1.7","mike7",expectedTimestamp, "sensor1");
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            var endpoints = dal.GetEndpoints(1,3);
            for(int i = 3; i < 6; i ++){
                Assert.Equal(i + "12345678901",endpoints[i-3].DeviceMacAddress);
                Assert.Equal("192.168.1."+i,endpoints[i-3].DeviceIpAddress);
                Assert.Equal("mike"+i,endpoints[i-3].DeviceName);
                Assert.True(expectedTimestamp - endpoints[i-3].TimeStamp < TimeSpan.FromSeconds(1));
                Assert.Equal("sensor1",endpoints[i-3].DetectedBy);
            }

            Assert.Equal(3, endpoints.Length);
        }

        [Fact]
        public void ShouldReturnSecondPageWithEndpointsIncomplete(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("012345678901","192.168.1.0","mike0",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("112345678901","192.168.1.1","mike1",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("212345678901","192.168.1.2","mike2",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("312345678901","192.168.1.3","mike3",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("412345678901","192.168.1.4","mike4",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("512345678901","192.168.1.5","mike5",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("612345678901","192.168.1.6","mike6",expectedTimestamp, "sensor1");
            dal.StoreDhcpEndpoint("712345678901","192.168.1.7","mike7",expectedTimestamp, "sensor1");
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            var endpoints = dal.GetEndpoints(1,5);
            for(int i = 5; i < 8; i ++){
                Assert.Equal(i + "12345678901",endpoints[i-5].DeviceMacAddress);
                Assert.Equal("192.168.1."+i,endpoints[i-5].DeviceIpAddress);
                Assert.Equal("mike"+i,endpoints[i-5].DeviceName);
                Assert.True(expectedTimestamp - endpoints[i-5].TimeStamp < TimeSpan.FromSeconds(1));
                Assert.Equal("sensor1",endpoints[i-5].DetectedBy);
            }

            Assert.Equal(3, endpoints.Length);
        }

        [Fact]
        public void ShouldReturnPageCount(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            dal.StoreDhcpEndpoint("012345678901","192.168.1.0","mike0",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("112345678901","192.168.1.1","mike1",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("212345678901","192.168.1.2","mike2",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("312345678901","192.168.1.3","mike3",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("412345678901","192.168.1.4","mike4",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("512345678901","192.168.1.5","mike5",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("612345678901","192.168.1.6","mike6",DateTime.Now, "sensor1");
            dal.StoreDhcpEndpoint("712345678901","192.168.1.7","mike7",DateTime.Now, "sensor1");

            var pageCount = dal.GetDhcpEndpointPageCount(2);
            Assert.Equal(4, pageCount);

            pageCount = dal.GetDhcpEndpointPageCount(5);
            Assert.Equal(2, pageCount);

            pageCount = dal.GetDhcpEndpointPageCount(10);
            Assert.Equal(1, pageCount);
        }

        [Fact]
        public void ShouldReturnEndpointsWithVendorName(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            dal.StoreDhcpEndpoint("00C0EE221133","192.168.1.1","mike1",DateTime.Now, "sensor1"); //KYOCERA Display Corporation
            dal.StoreDhcpEndpoint("CC5A532A3FE3","192.168.1.1","mike2",DateTime.Now, "sensor1"); //Cisco Systems, Inc
            dal.StoreDhcpEndpoint("949990123332","192.168.1.1","mike3",DateTime.Now, "sensor1"); //VTC Telecommunications
            dal.StoreDhcpEndpoint("10DDB1923801","192.168.1.1","mike4",DateTime.Now, "sensor1"); //Apple, Inc.
            var endpoints = dal.GetEndpoints(0,10);
            foreach(var e in endpoints){
                if(e.DeviceMacAddress == "00C0EE221133"){
                    Assert.Equal("KYOCERA Display Corporation", e.VendorName );
                }
                
                if(e.DeviceMacAddress == "CC5A532A3FE3"){
                    Assert.Equal("Cisco Systems, Inc", e.VendorName );
                }
                
                if(e.DeviceMacAddress == "949990123332"){
                    Assert.Equal("VTC Telecommunications", e.VendorName);
                }
                
                if(e.DeviceMacAddress == "10DDB1923801"){
                    Assert.Equal("Apple, Inc.", e.VendorName);
                }
            }
        }
    }
}
