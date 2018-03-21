namespace Ajakka.Alerting{
    public interface IAlertingStorage{
        void Load(string fileName);
        void Save(string fileName);
    }
}