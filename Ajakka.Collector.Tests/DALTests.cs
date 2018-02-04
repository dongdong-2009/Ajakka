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
            dal.StoreDhcpEndpoint("0123456","192.168.1.1","mike",expectedTimestamp);
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
                        Assert.Equal("0123456", mac);
                        Assert.Equal("192.168.1.1", ip);
                        Assert.Equal("mike", hostname);
                        Assert.True((expectedTimestamp - timestamp) < TimeSpan.FromSeconds(1));
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateEndpoint()
        {
            ClearEndpointsTable();

            ICollectorDAL dal = new DAL(testDbConnectionString);
            dal.StoreDhcpEndpoint("0123456","192.168.1.1","mike",DateTime.Now.AddDays(-1));
           
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("0123456","192.168.1.13","mike2",expectedTimestamp);
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
                        Assert.Equal("0123456", mac);
                        Assert.Equal("192.168.1.13", ip);
                        Assert.Equal("mike2", hostname);
                        Assert.True((expectedTimestamp - timestamp) < TimeSpan.FromSeconds(1));
                    }
                }
            }
        }
        
        [Fact]
        public void ShouldReturnFirstPageWithEndpoints(){
            ClearEndpointsTable();
            ICollectorDAL dal = new DAL(testDbConnectionString);
            var expectedTimestamp = DateTime.Now;
            dal.StoreDhcpEndpoint("012345678901","192.168.1.0","mike0",expectedTimestamp);
            dal.StoreDhcpEndpoint("112345678901","192.168.1.1","mike1",expectedTimestamp);
            dal.StoreDhcpEndpoint("212345678901","192.168.1.2","mike2",expectedTimestamp);
            dal.StoreDhcpEndpoint("312345678901","192.168.1.3","mike3",expectedTimestamp);
            dal.StoreDhcpEndpoint("412345678901","192.168.1.4","mike4",expectedTimestamp);
            dal.StoreDhcpEndpoint("512345678901","192.168.1.5","mike5",expectedTimestamp);
            dal.StoreDhcpEndpoint("612345678901","192.168.1.6","mike6",expectedTimestamp);
            dal.StoreDhcpEndpoint("712345678901","192.168.1.7","mike7",expectedTimestamp);
            expectedTimestamp = expectedTimestamp.ToUniversalTime();

            var endpoints = dal.GetEndpoints(0,5);
            for(int i = 0; i < 5; i ++){
                Assert.Equal(i + "12345678901",endpoints[i].DeviceMacAddress);
                Assert.Equal("192.168.1."+i,endpoints[i].DeviceIpAddress);
                Assert.Equal("mike"+i,endpoints[i].DeviceName);
                Assert.True(expectedTimestamp - endpoints[i].TimeStamp < TimeSpan.FromSeconds(1));
            }
        }
    }
}
