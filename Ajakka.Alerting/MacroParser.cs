using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ajakka.Alerting{
    public class MacroParser{

        public string Parse(string input, dynamic data){
            var properties = ((object)data).GetType().GetProperties().ToDictionary(p => p.Name, p=>p.GetValue(data));
            var macros = GetMacros(input);
            foreach(var item in properties){
                var macro = macros.FirstOrDefault((m)=>m == item.Key);
                if(macro != null){
                    input = input.Replace("{"+macro+"}", item.Value.ToString());
                }
            }
            return input;
        }

        private string[] GetMacros(string input){
            List<string> macros = new List<string>();

            if(string.IsNullOrEmpty(input))
                return new string[0];

            int startIndex = 0;
            int endIndex = 0;
            while(startIndex <= input.Length){
                startIndex = input.IndexOf('{', startIndex);
                if(startIndex == -1){
                    return macros.ToArray();
                }

                endIndex = input.IndexOf('}',startIndex);
                if(endIndex == -1){
                    return macros.ToArray();
                }
                string macro = input.Substring(startIndex + 1, endIndex - startIndex - 1);
                macros.Add(macro);
                startIndex++;
            }
            return macros.ToArray();
        }
    }
}