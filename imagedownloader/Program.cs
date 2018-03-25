using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace imagedownloader
{
    
    class Program
    {
        static List<string> failed = new List<string>();
        static readonly object failedSyncRoot = new object();
        static readonly object skippedSyncRoot = new object(); 
        static List<string> skipped = new List<string>();
        static readonly string TargetFolder = ".." + Path.DirectorySeparatorChar + "pictures" + Path.DirectorySeparatorChar;
        static readonly string InvalidCharacters = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        volatile static int processed = 0;
        static readonly object consoleSyncRoot = new object();

        static void Main(string[] args)
        {
            Console.WriteLine();
            SemaphoreSlim semaphore = new SemaphoreSlim(20);

            List<string> vendorNames = new List<string>();

            using (var stream = new FileStream("names.txt", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream)){
                    var line = "";
                    do{
                        line = reader.ReadLine();
                        if(!string.IsNullOrEmpty(line)){
                            vendorNames.Add(line);
                        }
                    }while(!string.IsNullOrEmpty(line));
                }

                foreach(var vendor in vendorNames)
                {
                    semaphore.Wait();
                    Thread thread = new Thread(()=>{
                        GetImage(vendor);
                        processed++;
                        lock(consoleSyncRoot){
                           Console.SetCursorPosition(0,Console.CursorTop);
                           Console.Write(processed + "/" + vendorNames.Count + "                ");
                        }
                        semaphore.Release();
                    });
                    thread.Start();
                }
                
                do{
                    Console.WriteLine("Waiting for downloads to finish...");
                    Thread.Sleep(100);
                }while(semaphore.CurrentCount < 10);
                
                Console.WriteLine("Failed:");
                foreach(var f in failed){
                    Console.WriteLine(f);
                }
            }
        }

        private static string GetImageUrl(string page, string vendor){
            string imageUrl = "";
            int lastFoundIndex = 0;
            do{
                int firstimagetagbegin = page.IndexOf("<a class=\"thumb\"", lastFoundIndex);
                if(firstimagetagbegin == -1)
                    return "";
                var innerstart = page.IndexOf("href=\"", firstimagetagbegin) + "href=\"".Length;
                var innerEnd = page.IndexOf("\"", innerstart);
                var length = innerEnd - innerstart;
                imageUrl = page.Substring(innerstart, length);
                var fileName = GetValidVendorFileName(imageUrl, vendor);
                if(IsExtensionAllowed(Path.GetExtension(fileName))){
                    return imageUrl;
                }
                else {
                    lastFoundIndex = firstimagetagbegin + 10;
                }
            }while(lastFoundIndex<page.Length);
            return imageUrl;
        }

        static bool IsExtensionAllowed(string extension){
            return extension == ".jpg" || extension == ".png";
        }

        private static void GetImage(string vendor)
        {
            var sq = vendor.Replace(' ', '+') + "+logo";
            string url = "https://www.bing.com/images/search?q="+sq+"&go=Submit&qs=n&form=QBLH&scope=images&pq=" +sq;
            WebClient client = new WebClient();
            var page = Encoding.UTF8.GetString(client.DownloadData(url));
            try{
                var imageUrl = GetImageUrl(page, vendor);
                
                if(!String.IsNullOrEmpty(imageUrl))
                {
                    var fileName = GetValidVendorFileName(imageUrl, vendor);

                    if(!string.IsNullOrEmpty(imageUrl))
                    {
                        DownloadImage(imageUrl, fileName, vendor);
                    }
                    else
                    {
                        lock(failedSyncRoot){
                            failed.Add(vendor);
                        }
                    }
                }
            }
            catch{
                lock(failedSyncRoot){
                    failed.Add(vendor);
                }
            }
        }

        private static string GetVendorName(string vendorName){
            vendorName  = vendorName.ToLower();
            
            var index = vendorName.IndexOf("(");
            
            if(index > 0){
                vendorName = vendorName.Substring(0,index);
            }
            vendorName = vendorName.Replace(",","");
            vendorName = vendorName.Replace(".","");    
            vendorName = vendorName.Replace(" ","");
            vendorName = RemoveVendorNameSuffix(vendorName,"coltd");

            vendorName = RemoveVendorNameSuffix(vendorName,"co");

            vendorName = RemoveVendorNameSuffix(vendorName,"gmbh");

            vendorName = RemoveVendorNameSuffix(vendorName,"inc");
            return vendorName; 
        }

        static string RemoveVendorNameSuffix(string vendorName,string suffix){
            if(vendorName.EndsWith(suffix)){
                vendorName = vendorName.Substring(0,vendorName.Length - suffix.Length);
            }
            return vendorName;
        }

        private static string GetValidImageExtension(string imageUrl){
            var ext = Path.GetExtension(imageUrl);
           
            if (ext.Length > 4)
                ext = ext.Substring(0, 4);
            foreach (char c in InvalidCharacters)
            {
                ext = ext.Replace(c.ToString(), "");
            }
            return ext;
        }
        private static string GetValidVendorFileName(string imageUrl, string vendor){
            var ext = GetValidImageExtension(imageUrl);
            vendor = vendor.Replace('/', '_').Replace('\\', '_');
            foreach (char c in InvalidCharacters)
            {
                vendor = vendor.Replace(c.ToString(), "");
            }
            return Path.Combine(TargetFolder, GetVendorName(vendor) + ext);
        }
        private static void DownloadImage(string imageUrl, string targetFileName, string vendor)
        {
            if(File.Exists(targetFileName)){
                lock(skipped){
                    skipped.Add(vendor);
                }
                return;
            }

            try
            {
                WebClient client = new WebClient();
                var data = client.DownloadData(imageUrl);
               
                using (var fs = new FileStream(targetFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            { 
                lock(failedSyncRoot){
                    failed.Add(vendor);
                }
            }
        }
    }
}
