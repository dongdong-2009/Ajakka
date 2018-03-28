using System;
using System.Collections.Generic;
using System.Data;
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
                RunScript(connection,"usersettings.sql");
                if(RunScript(connection,"vendors.sql")){
                    Console.WriteLine("Do you want to import vendor names? [y/n]");
                    var key = Console.ReadKey();
                    Console.WriteLine();
                    if (key.KeyChar == 'y' || key.KeyChar == 'Y'){
                        try{
                            OuiDownloader.DownloadOuiList("oui.txt");
                            var vendors = OuiParser.Parse("oui.txt");
                            InsertVendors(connection, vendors);
                        }
                        catch(Exception ex){
                            LogError("Error: Could not finish creating vendors table.", ex.Message);
                        }
                    }
                }
                Console.WriteLine("Database setup finished.");
            }
        }

        static void InsertVendors(MySqlConnection connection, List<Tuple<string,string>> vendors){
            Console.WriteLine("Adding vendor information to database" + Environment.NewLine);
            for(int i = 0; i < vendors.Count; i++){
                var cmd = connection.CreateCommand();
                cmd.CommandText="insert into vendors values (@oui, @name)";
                cmd.Parameters.Add("@oui", DbType.StringFixedLength).Value=vendors[i].Item1;
                cmd.Parameters.Add("@name", DbType.String).Value=vendors[i].Item2;
                try{
                    cmd.ExecuteNonQuery();
                    Console.SetCursorPosition(0,Console.CursorTop -1);
                    Console.WriteLine((i * 100 / vendors.Count) + " % complete ");
                }
                catch(Exception ex){
                    LogError("Failed to insert vendor " + vendors[i].Item2 + "(" + vendors[i].Item1 + ")",ex.Message);
                    Console.WriteLine();
                }
            }
        }

        static string LoadCommand(string fileName){
            using(var reader = new StreamReader(fileName)){
                return reader.ReadToEnd();
            }
        }

        static bool RunScript(MySqlConnection connection, string fileName){
            var defaultColor = Console.ForegroundColor;
            try{
                var command = connection.CreateCommand();
                command.CommandText = LoadCommand(fileName);
                command.ExecuteNonQuery();
                LogSuccess(fileName,"");
                return true;
                
            }
            catch(Exception ex){
                LogError(fileName, ex.Message);
                return false;
            }
        }

        static void LogSuccess(string title, string additionalInfo){
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SUCCESS: " + title);
            Console.ForegroundColor = defaultColor;
            if(!string.IsNullOrEmpty(additionalInfo)){
                Console.WriteLine(additionalInfo);
            }
        }
        static void LogError(string title, string additionalInfo){
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + title);
            Console.ForegroundColor = defaultColor;
            Console.WriteLine(additionalInfo);
        }
    }
}
