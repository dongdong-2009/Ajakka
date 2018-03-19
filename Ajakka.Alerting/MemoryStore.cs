using System.Collections.Generic;

namespace Ajakka.Alerting{
    public class MemoryStore : IActionStore
    {
        static int lastId = 0;
        const int pageSize = 10;

        readonly Dictionary<int, AlertActionBase> alertActions = new Dictionary<int, AlertActionBase>();

        static int GetIdAndIncrement(){
            return lastId++;
        }

        public AlertActionBase AddAction(AlertActionBase action)
        {
            var add = (AlertActionBase)action.Clone();
            add.Id = GetIdAndIncrement();
            alertActions.Add(add.Id,add);
            return add;
        }

        public void DeleteAction(int actionId)
        {
            alertActions.Remove(actionId);
        }

        public AlertActionBase GetAction(int actionId)
        {
            var action = alertActions[actionId];
            action.Initialize();
            return action;
        }

        public AlertActionBase[] GetActions(int pageNumber)
        {
            var startIndex = pageSize *pageNumber;
            if(startIndex > alertActions.Values.Count){
                return new AlertActionBase[]{};
            }
            var values = new AlertActionBase[alertActions.Values.Count];
            alertActions.Values.CopyTo(values,0);
            var targetSize = pageSize;
            if(startIndex + pageSize > values.Length){
                targetSize = values.Length - startIndex;
            }
            var ret = new AlertActionBase[targetSize];
            for(int i = startIndex, j = 0;i<startIndex + targetSize ;i++, j++ ){
                ret[j] = values[i];
                ret[j].Initialize();
            }
            return ret;
        }

        public int GetPageCount()
        {
            int add = 0;
            if (alertActions.Count % pageSize > 0){
                add = 1;
            }
            return add + alertActions.Count/pageSize;
        }

        public void UpdateAction(int actionId, AlertActionBase update)
        {
            alertActions[actionId] = (AlertActionBase)update.Clone();
        }
    }
}