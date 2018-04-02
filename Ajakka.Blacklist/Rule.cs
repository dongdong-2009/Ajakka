using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Ajakka.Blacklist{
    [DataContract]
    public class Rule:ICloneable{

        private List<int> alertActionIds = new List<int>();

        [DataMember]
        public Guid Id {get;set;}
        [DataMember]
        public string Name {get;set;}
        [DataMember]
        public string Pattern {get;set;}
      
        public Rule(){

        }

        public Rule(string name, string pattern){
            Name = name;
            Pattern = pattern;
        }

        public object Clone()
        {
           var rule = new Rule{
                Id = this.Id,
                Name = this.Name,
                Pattern = this.Pattern
            };
            return rule;
        }

        public bool IsMatch(string input){
            if(string.IsNullOrEmpty(input)){
                return false;
            }
            var regex = new Regex(Pattern);
            return regex.IsMatch(input);
        }
    }
}

    