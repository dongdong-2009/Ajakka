using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Ajakka.Blacklist{
    class MemoryBlacklist:IBlacklist, IBlacklistStorage{
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

        public void Load(string fileName)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<Rule>));
            using(var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)){
                List<Rule> loaded = (List<Rule>)serializer.ReadObject(stream);
                rules.Clear();
                foreach(var l in loaded){
                    rules.Add(l.Id, l);
                }
            }
        }

        public void Save(string fileName)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<Rule>));
            using(var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)){
                var list = new List<Rule>();
                list.AddRange(rules.Values);
                serializer.WriteObject(stream, list);  
                    
            }
            
        }
    }
}