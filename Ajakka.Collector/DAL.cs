
using System;
using System.Data;

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
    }
}