using System;
using System.IO;
using MySql.Data.MySqlClient;

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
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                RunScript(connection,"endpointLatest.sql");
                RunScript(connection,"users.sql");
            }
        }

        static string LoadCommand(string fileName){
            using(var reader = new StreamReader(fileName)){
                return reader.ReadToEnd();
            }
        }

        static void RunScript(MySqlConnection connection, string fileName){
            var defaultColor = Console.ForegroundColor;
            try{
                var command = connection.CreateCommand();
                command.CommandText = LoadCommand(fileName);
                command.ExecuteNonQuery();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("SUCCESS: ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine(fileName);
                
            }
            catch(Exception ex){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("FAILURE: ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine(fileName);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
