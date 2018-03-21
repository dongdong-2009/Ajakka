namespace Ajakka.Blacklist{
    public interface IBlacklistStorage{
        void Load(string fileName);
        void Save(string fileName);
    }
}