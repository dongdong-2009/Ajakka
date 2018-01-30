using System;
using Xunit;
//https://blogs.msdn.microsoft.com/dotnet/2016/11/09/net-core-data-access/
namespace Ajakka.Collector.Tests
{
    public class DALTests
    {
        [Fact]
        public void Test1()
        {
            using (var connection = new SqliteConnection("Filename=" + path))
            {
                connection.Open();

                using (var reader = connection.ExecuteReader("SELECT Name FROM Person;"))
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Hello {reader.GetString(0)}!"));
                    }
                }
            }
        }
    }
}
