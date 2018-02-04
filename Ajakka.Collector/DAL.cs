
using System;
using System.Data;
using System.Collections.Generic;

namespace Ajakka.Collector{
    public class DAL:ICollectorDAL{
        readonly string connectionString;
        private DAL(){}

        public DAL(string connectionString){
            this.connectionString = connectionString;
        }

        public void StoreDhcpEndpoint(string mac, string ip, string hostname, DateTime timestamp){
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO endpoint_latest(mac,ip,hostname,lastseen) VALUES(@mac, @ip, @hostname,@lastseen) ON DUPLICATE KEY UPDATE ip=@ip, hostname=@hostname, lastseen=@lastseen;";
                command.Parameters.Add("@mac", DbType.StringFixedLength).Value=mac;
                command.Parameters.Add("@ip", DbType.String).Value=ip;
                command.Parameters.Add("@hostname", DbType.String).Value = hostname;
                command.Parameters.Add("@lastseen", DbType.DateTime).Value = timestamp.ToUniversalTime();
                command.ExecuteNonQuery();
                
            }
        }

        public EndpointDescriptor[] GetEndpoints(int pageNumber, int pageSize){
            List<EndpointDescriptor> result = new List<EndpointDescriptor>();

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
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
                        result.Add(new EndpointDescriptor{
                            DeviceName = hostname,
                            DeviceMacAddress = mac,
                            DeviceIpAddress = ip,
                            TimeStamp = timestamp
                        });
                    }
                }
            }
            return result.ToArray();
        }
    }
}