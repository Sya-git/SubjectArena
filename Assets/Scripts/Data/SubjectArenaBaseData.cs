using UnityEngine;

namespace SubjectArena.Data
{
    public class SubjectArenaBaseData : ScriptableObject
    {
        [SerializeField] private string guid;
        
        public string Guid => guid;
        
        #if UNITY_EDITOR
        
        public void EditorSetGuid(string guid)
        {
            this.guid = guid;
        }
        
        #endif
    }
}