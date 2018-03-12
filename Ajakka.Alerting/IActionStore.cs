namespace Ajakka.Alerting{
    public interface IActionStore{
        AlertActionBase[] GetActions(int pageNumber);
        AlertActionBase AddAction(AlertActionBase action);
        void DeleteAction(int actionId);
        void UpdateAction(int actionId, AlertActionBase update);
        AlertActionBase GetAction(int actionId);
        int GetPageCount();
    }
}