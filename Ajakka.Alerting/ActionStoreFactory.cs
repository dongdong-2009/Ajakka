namespace Ajakka.Alerting{
    public class ActionStoreFactory{
        public static IActionStore GetActionStore(){
            return new MemoryStore();
        }
    }
}