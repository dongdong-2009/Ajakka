using System;
using System.IO;
using System.Net;

namespace Ajakka.DbInit{
    class OuiDownloader{

        public static void DownloadOuiList(string targetFileName)
        {
            if(File.Exists(targetFileName)){
                Console.WriteLine("local file " + targetFileName + " exists. Download a new one? Y/N");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 'n' || key.KeyChar == 'N'){
                    Console.WriteLine("Download skipped");
                    return;
                }
            }
            var client = new WebClient();
            var buffer = new byte[10*1024];
            var totalRead = 0L;
            Console.WriteLine("Attempting to download oui.txt");
            using (var stream = client.OpenRead("http://standards.ieee.org/develop/regauth/oui/oui.txt")){
                Console.WriteLine("Downloading oui.txt from http://standards.ieee.org/develop/regauth/oui/oui.txt" + Environment.NewLine);
                using (var outStream = new FileStream(targetFileName,FileMode.Create, FileAccess.Write)){
                    using(var writer = new BinaryWriter(outStream)){
                        int bytesRead = 0;
                        do{
                            bytesRead = stream.Read(buffer,0,buffer.Length);
                            writer.Write(buffer,0,bytesRead);
                            totalRead += bytesRead;
                            Console.SetCursorPosition(0,Console.CursorTop - 1);
                            Console.Write(totalRead + " bytes downloaded");
                            var max = 3860408L; //estimated stream length
                            var completed = totalRead * 100/max;
                            Console.WriteLine(" ("+completed+" %)");
                        }while(bytesRead > 0);
                    }
                }
            }
            
        }
    }
}