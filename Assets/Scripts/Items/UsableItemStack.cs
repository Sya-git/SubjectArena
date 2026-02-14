using System;

namespace SubjectArena.Items
{
    [Serializable]
    public struct UsableItemStack
    {
        public UsableItemData ItemData;
        public int Quantity;

        public UsableItemStack(UsableItemData itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;
        }

        public bool IsEmpty => ItemData == null || Quantity <= 0;
        public int FreeSpace => ItemData != null ? ItemData.MaxStack - Quantity : 0;
        public bool CanAdd(int amount) => ItemData != null && amount <= FreeSpace;
        
        public bool CanConsume => Quantity > 0;
        public bool Consume()
        {
            if (Quantity <= 0)
                return false;

            Quantity--;
            return true;
        }
    }
}