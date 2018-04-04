using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Ajakka.Alerting{
    public class MemoryStore : IActionStore, IAlertingStorage
    {
        int lastId = 0;
        const int pageSize = 10;

        readonly Dictionary<int, AlertActionBase> alertActions = new Dictionary<int, AlertActionBase>();
        readonly Dictionary<Guid, List<int>> ruleToActionMap = new Dictionary<Guid, List<int>>();

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
            var serializer = new DataContractJsonSerializer(typeof(MemoryStoreContainer));
            using(var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)){
                MemoryStoreContainer loaded = (MemoryStoreContainer)serializer.ReadObject(stream);
                alertActions.Clear();
                foreach (var item in loaded.AlertActions){
                    alertActions.Add(item.Key, item.Value);
                }
                ruleToActionMap.Clear();
                foreach(var item in loaded.RuleToActionMap){
                    ruleToActionMap.Add(item.Key, item.Value);
                }
                lastId = loaded.LastId;
            }
        }

        public void Save(string fileName)
        {
            var serializer = new DataContractJsonSerializer(typeof(MemoryStoreContainer));
            using(var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)){
                var container = new MemoryStoreContainer();
                container.AlertActions = this.alertActions;
                container.RuleToActionMap = this.ruleToActionMap;
                container.LastId = this.lastId;
                serializer.WriteObject(stream, container);  
            }
        }

        public void LinkRuleToAction(Guid ruleId, int id)
        {
            if(!ruleToActionMap.ContainsKey(ruleId)){
                ruleToActionMap.Add(ruleId, new List<int>());
            }
            if(!ruleToActionMap[ruleId].Contains(id)){
                ruleToActionMap[ruleId].Add(id);
            }
        }

        public AlertActionBase[] GetLinkedActions(Guid ruleId)
        {
            List<int> list;
            if(ruleToActionMap.TryGetValue(ruleId, out list)){
                var actions = new AlertActionBase[list.Count];
                for(int i = 0; i < list.Count; i ++){
                    actions[i] = GetAction(list[i]);
                }
                return actions;
            }
            return new AlertActionBase[0];
        }

        public void DeleteRuleAndActions(Guid ruleId)
        {
            foreach(var action in GetLinkedActions(ruleId)){
                DeleteAction(action.Id);
            }
            ruleToActionMap.Remove(ruleId);
        }
    }
}