using System;
using System.Collections;

namespace Ajakka.Blacklist{
    public interface IBlacklist{
        Rule AddRule(Rule rule);
        IEnumerable GetRules();
        void DeleteRule(Guid id);
        void UpdateRule(Guid id, Rule rule);
        Rule GetRule(Guid id);
    }
}