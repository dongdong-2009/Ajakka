using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ajakka.Blacklist{
    [DataContract]
    public class Rule:ICloneable{

        private readonly List<int> alertActionIds = new List<int>();

        [DataMember]
        public Guid Id {get;set;}
        [DataMember]
        public string Name {get;set;}
        [DataMember]
        public string Pattern {get;set;}
        [DataMember]
        public List<int> AlertActionIds {get {return alertActionIds; }}

        public object Clone()
        {
           var rule = new Rule{
                Id = this.Id,
                Name = this.Name,
                Pattern = this.Pattern
            };
            rule.AlertActionIds.AddRange(this.AlertActionIds);
            return rule;
        }
    }
}

    