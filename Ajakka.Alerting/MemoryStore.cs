using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Ajakka.Alerting{
    public class MemoryStore : IActionStore, IAlertingStorage
    {
        int lastId = 0;
        const int pageSize = 10;

        readonly Dictionary<int, AlertActionBase> alertActions = new Dictionary<int, AlertActionBase>();

        int GetIdAndIncrement(){
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

        public void Load(string fileName)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<AlertActionBase>));
            using(var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)){
                List<AlertActionBase> loaded = (List<AlertActionBase>)serializer.ReadObject(stream);
                alertActions.Clear();
                foreach(var l in loaded){
                    alertActions.Add(l.Id, l);
                    if(lastId <= l.Id){
                        lastId = l.Id+1;
                    }
                }
            }
        }

        public void Save(string fileName)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<AlertActionBase>));
            using(var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)){
                var list = new List<AlertActionBase>();
                list.AddRange(alertActions.Values);
                serializer.WriteObject(stream, list);  
                    
            }
        }
    }
}