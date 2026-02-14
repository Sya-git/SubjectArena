using SubjectArena.Data;
using UnityEngine;

namespace SubjectArena.Items.Data
{
    public abstract class UsableItemData : SubjectArenaBaseData
    {
        [SerializeField] private string itemName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxStack = 99;
        [SerializeField] private GroundItem groundItemPrefab;

        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MaxStack => maxStack;
        public GroundItem GroundItemPrefab => groundItemPrefab;
        
        public abstract bool Use(GameObject user);
    }
}