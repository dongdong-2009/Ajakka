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
    }
}
