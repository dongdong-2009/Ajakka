using System;
using System.Collections;
using System.Collections.Generic;

namespace Ajakka.Blacklist{
    class MemoryBlacklist:IBlacklist{
        readonly Dictionary<Guid, Rule> rules = new Dictionary<Guid, Rule>();

        public Rule AddRule(Rule rule){
            if(string.IsNullOrEmpty(rule.Name)){
                throw new ArgumentException("rule name cannot be empty");
            }
            var id = Guid.NewGuid();
            var add = (Rule)rule.Clone();
            add.Id = id;
            rules.Add(id,add);
            return add;
        }
        public IEnumerable GetRules(){
            throw new NotImplementedException();
        }
        public void DeleteRule(Guid id){
            throw new NotImplementedException();
        }
        public void UpdateRule(Guid id, Rule rule){
            throw new NotImplementedException();
        }
        public Rule GetRule(Guid id){
            return rules[id];
        }
    }
}