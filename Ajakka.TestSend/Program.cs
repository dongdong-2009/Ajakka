using System;
using System.IO;

namespace Ajakka.TestSend
{
    public class Program
    {

        public static void Main(string[] args)
        {
            ShowHelp();
            var command = "";
            var sendCommandProcessor = new SendCommandProcessor();
            do{
                command = Console.ReadLine();
                if(command.ToLower().StartsWith("s"))
                {
                    sendCommandProcessor.ProcessSendCommand(command);
                }
                if(command.ToLower().StartsWith("h")){
                    ShowHelp();
                }
                if(command.ToLower().StartsWith("c")){
                    Console.Clear();
                }
            }while(command.ToLower() != "x");
        }

        static void ShowHelp(){
            using(var reader = new StreamReader("help.txt")){
                Console.WriteLine(reader.ReadToEnd());
            }
        }
    }
}
