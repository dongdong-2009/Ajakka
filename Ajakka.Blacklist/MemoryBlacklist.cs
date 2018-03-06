using System;
using System.Collections;
using System.Collections.Generic;

namespace Ajakka.Blacklist{
    class MemoryBlacklist:IBlacklist{
        readonly Dictionary<Guid, Rule> rules = new Dictionary<Guid, Rule>();
        const int pageSize = 10;

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
        public Rule[] GetRules(int pageNumber){
            var startIndex = pageSize *pageNumber;
            if(startIndex > rules.Values.Count){
                return new Rule[]{};
            }
            var values = new Rule[rules.Values.Count];
            rules.Values.CopyTo(values,0);
            var targetSize = pageSize;
            if(startIndex + pageSize > values.Length){
                targetSize = values.Length - startIndex;
            }
            var ret = new Rule[targetSize];
            for(int i = startIndex, j = 0;i<startIndex + targetSize ;i++, j++ ){
                ret[j] = values[i];
            }
            return ret;
        }
        public void DeleteRule(Guid id){
            if(rules.ContainsKey(id)){
                rules.Remove(id);
                return;
            }
            throw new InvalidOperationException("A rule with this id does not exist.");
        }
        public Rule UpdateRule(Guid id, Rule rule){
            if(string.IsNullOrEmpty(rule.Name)){
                throw new ArgumentException("rule.Name cannot be empty");
            }
            if(rules.ContainsKey(id)){
                rules[id] = rule;
                rules[id].Id = id;
                return rules[id];
            }
            throw new InvalidOperationException("A rule with this id does not exist.");
        }
        public Rule GetRule(Guid id){
            Rule rule;
            if(rules.TryGetValue(id,out rule))
            {
                return rule;
            }
            throw new InvalidOperationException("A rule with this id does not exist.");
        }

        public int GetRulePageCount(){
            int add = 0;
            if (rules.Count % pageSize > 0){
                add = 1;
            }
            return add + rules.Count/pageSize;
        }
    }
}