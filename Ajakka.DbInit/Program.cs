using System;
using System.IO;

namespace Ajakka.DbInit
{
    class Program
    {
        static string connectionString;
        const string defaultConnectionStringName = "AjakkaConnection";

        static void Main(string[] args)
        {
            connectionString = Environment.GetEnvironmentVariable("AjakkaConnection");
            if(args.Length > 0){
                connectionString = Environment.GetEnvironmentVariable(args[0]);
            }
            if(string.IsNullOrEmpty(connectionString)){
                Console.WriteLine("Connection string is empty.");
                return;
            }
            CreateEndpointLatestTable();
        }

        static void CreateEndpointLatestTable(){
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = LoadCommand("endpointLatest.sql");
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
            }
        }

        static string LoadCommand(string fileName){
            using(var reader = new StreamReader(fileName)){
                return reader.ReadToEnd();
            }
        }
    }
}
