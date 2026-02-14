using UnityEngine;

namespace SubjectArena.Items
{
    public abstract class UsableItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxStack = 99;

        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MaxStack => maxStack;
        
        public abstract bool Use(GameObject user);
    }
}