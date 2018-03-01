namespace Ajakka.Blacklist{
    public class BlacklistFactory{
        public static IBlacklist CreateBlacklist(){
            return new MemoryBlacklist();
        }
    }
}