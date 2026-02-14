using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectArena.Data
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private SubjectArenaBaseData[] datas;

        private readonly Dictionary<string, SubjectArenaBaseData> _datasByGuid = new();
        private static DataManager _instance;
        private void Awake()
        {
            foreach (var data in datas)
            {
                if (_datasByGuid.ContainsKey(data.Guid))
                {
                    Debug.LogError(string.Concat("Duplicate data guid: ", data.Guid));
                }
                _datasByGuid.Add(data.Guid, data);
            }

            _instance = this;
        }

        public static T GetDataByGuid<T>(in string guid) where T : SubjectArenaBaseData
        {
            if (_instance._datasByGuid.TryGetValue(guid, out var data))
            {
                return data as T;
            }

            return null;
        }
    }
}