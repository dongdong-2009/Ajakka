using System;

namespace Ajakka.Alerting{
    public interface IActionStore{
        AlertActionBase[] GetActions(int pageNumber);
        AlertActionBase AddAction(AlertActionBase action);
        void DeleteAction(int actionId);
        void UpdateAction(int actionId, AlertActionBase update);
        AlertActionBase GetAction(int actionId);
        int GetPageCount();
        void LinkRuleToAction(Guid ruleId, int id);
        AlertActionBase[] GetLinkedActions(Guid ruleId);
        void DeleteRuleAndActions(Guid ruleId);
    }
}