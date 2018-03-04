using System;
using System.Collections;

namespace Ajakka.Blacklist{
    public interface IBlacklist{
        Rule AddRule(Rule rule);
        Rule[] GetRules(int pageNumber);
        void DeleteRule(Guid id);
        Rule UpdateRule(Guid id, Rule rule);
        Rule GetRule(Guid id);
    }
}