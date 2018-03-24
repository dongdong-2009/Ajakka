using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Ajakka.DbInit{
    class OuiParser{
        

        internal static List<Tuple<string,string>> Parse(string fileName)
        {
            string text;
            using(var reader = new StreamReader(fileName))
            {
                text = reader.ReadToEnd();
            }
            
            var regex = new Regex(@"[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F] *\(base 16\) *.\t.*");
            var matches = regex.Matches(text);

            var vendors = new List<Tuple<string,string>>();
            foreach (var match in matches)
            {
                var parts = Regex.Split(match.ToString(), @"\(base 16\)");
                var vendor = new Tuple<string,string>(parts[0].Trim(),parts[1].Trim());
                vendors.Add(vendor); 
            }
            return vendors;
        }

    }
}