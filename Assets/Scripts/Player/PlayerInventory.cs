using System;
using SubjectArena.Data;
using SubjectArena.Items;
using SubjectArena.Items.Data;
using UnityEngine;

namespace SubjectArena.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int usableSlotsAmount = 1;
        [SerializeField] private int bagSlotsAmount = 3;

        //public UsableItemStack[] UsableSlots { get; private set; }
        //public UsableItemStack[] BagSlots { get; private set; }
        public UsableItemStack[] UsableSlots;
        public UsableItemStack[] BagSlots;

        public event Action<SlotType, int> OnSlotChanged;

        public enum SlotType : byte
        {
            Usable,
            Bag
        }

        private void Awake()
        {
            UsableSlots = new UsableItemStack[usableSlotsAmount];
            BagSlots = new UsableItemStack[bagSlotsAmount];
        }

        public bool GetItemAtSlot(SlotType slotType, int slotIndex, out UsableItemStack itemStack)
        {
            itemStack = default;
            if (slotIndex < 0)
                return false;
            if (slotIndex >= (slotType == SlotType.Usable ? UsableSlots.Length : BagSlots.Length))
                return false;
            
            itemStack = slotType == SlotType.Usable ? UsableSlots[slotIndex] : BagSlots[slotIndex];
            return true;
        }

        private ref UsableItemStack GetItemRefAtSlot(SlotType slotType, int slotIndex)
        {
            if (slotIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            if (slotIndex >= (slotType == SlotType.Usable ? UsableSlots.Length : BagSlots.Length))
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            return ref (slotType == SlotType.Usable ? ref UsableSlots[slotIndex] : ref BagSlots[slotIndex]);
        }

        public bool UseItemAtSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= usableSlotsAmount)
                return false;

            if (!UsableSlots[slotIndex].CanConsume)
                return false;

            var itemData = UsableSlots[slotIndex].ItemData;

            if (!itemData.Use(gameObject))
                return false;

            UsableSlots[slotIndex].Consume();

            if (UsableSlots[slotIndex].Quantity <= 0)
                UsableSlots[slotIndex] = default;

            OnSlotChanged?.Invoke(SlotType.Usable, slotIndex);
            return true;
        }

        public bool DestroyItemAtSlot(SlotType slotType, int slotIndex)
        {
            try
            {
                ref var itemSlot = ref GetItemRefAtSlot(slotType, slotIndex);
                itemSlot = default;
                OnSlotChanged?.Invoke(slotType, slotIndex);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public int AddItem(UsableItemData item, int quantity)
        {
            var remaining = AddToSlots(UsableSlots, item, quantity, SlotType.Usable);
            if (remaining > 0)
                remaining = AddToSlots(BagSlots, item, remaining, SlotType.Bag);
            return remaining;
        }
        
        public bool CanAddItem(UsableItemData item, int quantity)
        {
            var remaining = CalculateSpaceInSlots(UsableSlots, item, quantity);
            if (remaining > 0)
                remaining = CalculateSpaceInSlots(BagSlots, item, remaining);
            return remaining <= 0;
        }
        
        public int AddItemToBag(UsableItemData item, int quantity)
        {
            return AddToSlots(BagSlots, item, quantity, SlotType.Bag);
        }
        
        public bool CanAddItemToBag(UsableItemData item, int quantity)
        {
            return CalculateSpaceInSlots(BagSlots, item, quantity) <= 0;
        }
        
        public int AddItemToUsable(UsableItemData item, int quantity)
        {
            return AddToSlots(UsableSlots, item, quantity, SlotType.Usable);
        }
        
        public bool CanAddItemToUsable(UsableItemData item, int quantity)
        {
            return CalculateSpaceInSlots(UsableSlots, item, quantity) <= 0;
        }
        
        public bool MoveUsableToBag(int usableSlotIndex)
        {
            if (usableSlotIndex < 0 || usableSlotIndex >= usableSlotsAmount)
                return false;

            ref var sourceSlot = ref UsableSlots[usableSlotIndex];
            if (sourceSlot.IsEmpty)
                return false;

            // Verifica se cabe tudo na bag antes de mover
            if (!CanAddItemToBag(sourceSlot.ItemData, sourceSlot.Quantity))
                return false;

            AddToSlots(BagSlots, sourceSlot.ItemData, sourceSlot.Quantity, SlotType.Bag);
            sourceSlot = default;
            OnSlotChanged?.Invoke(SlotType.Usable, usableSlotIndex);
            return true;
        }

        public bool MoveBagToUsable(int bagSlotIndex)
        {
            if (bagSlotIndex < 0 || bagSlotIndex >= bagSlotsAmount)
                return false;

            ref var sourceSlot = ref BagSlots[bagSlotIndex];
            if (sourceSlot.IsEmpty)
                return false;

            var item = sourceSlot.ItemData;
            var originalQuantity = sourceSlot.Quantity;

            var remaining = AddToSlots(UsableSlots, item, originalQuantity, SlotType.Usable);

            // Nada foi movido
            if (remaining == originalQuantity)
                return false;

            // Atualiza o slot da bag com o que sobrou
            if (remaining > 0)
            {
                sourceSlot.Quantity = remaining;
            }
            else
            {
                sourceSlot = default;
            }

            OnSlotChanged?.Invoke(SlotType.Bag, bagSlotIndex);
            return true;
        }

        public bool CanSwapSlots(SlotType fromSlotType, SlotType toSlotType, int fromSlotIndex, int toSlotIndex)
        {
            try
            {
                if (fromSlotType == toSlotType && fromSlotIndex == toSlotIndex)
                    return false; // Do not swap same slot

                if (fromSlotIndex < 0 || toSlotIndex < 0)
                    return false;

                var fromSlot = GetItemRefAtSlot(fromSlotType, fromSlotIndex);
                var toSlot = GetItemRefAtSlot(toSlotType, toSlotIndex);

                if (fromSlot.IsEmpty)
                    return false; // Do not swap FROM empty slots

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SwapSlots(SlotType fromSlotType, SlotType toSlotType, int fromSlotIndex, int toSlotIndex)
        {
            if (!CanSwapSlots(fromSlotType, toSlotType, fromSlotIndex, toSlotIndex))
                return false;
            
            ref var fromSlot = ref GetItemRefAtSlot(fromSlotType, fromSlotIndex);
            ref var toSlot = ref GetItemRefAtSlot(toSlotType, toSlotIndex);

            // (fromSlotType == SlotType.Usable ? UsableSlots : BagSlots)[fromSlotIndex] = toSlot;
            // (toSlotType == SlotType.Usable ? UsableSlots : BagSlots)[toSlotIndex] = fromSlot;
            
            (fromSlot, toSlot) = (toSlot, fromSlot);
            
            OnSlotChanged?.Invoke(fromSlotType, fromSlotIndex);
            OnSlotChanged?.Invoke(toSlotType, toSlotIndex);

            return true;
        }
        
        private int AddToSlots(UsableItemStack[] slots, UsableItemData item, int quantity, SlotType type)
        {
            var remaining = quantity;
            
            for (var i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (slots[i].ItemData != item) continue;

                var spaceLeft = item.MaxStack - slots[i].Quantity;
                if (spaceLeft <= 0) continue;

                var toAdd = Mathf.Min(remaining, spaceLeft);
                slots[i].Quantity += toAdd;
                remaining -= toAdd;
                OnSlotChanged?.Invoke(type, i);
            }

            // Usa slots vazios
            for (var i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (!slots[i].IsEmpty) continue;

                var toAdd = Mathf.Min(remaining, item.MaxStack);
                slots[i] = new UsableItemStack(item, toAdd);
                remaining -= toAdd;
                OnSlotChanged?.Invoke(type, i);
            }

            return remaining;
        }

        /// <summary>
        /// Calcula quanto NÃO caberia num array de slots, sem modificar nada.
        /// Mesma lógica do AddToSlots mas read-only.
        /// </summary>
        private static int CalculateSpaceInSlots(UsableItemStack[] slots, UsableItemData item, int quantity)
        {
            var remaining = quantity;

            // Espaço em slots existentes com o mesmo item
            for (var i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (slots[i].ItemData != item) continue;

                var spaceLeft = item.MaxStack - slots[i].Quantity;
                remaining -= Mathf.Min(remaining, spaceLeft);
            }

            // Espaço em slots vazios
            for (var i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (!slots[i].IsEmpty) continue;

                remaining -= Mathf.Min(remaining, item.MaxStack);
            }

            return remaining;
        }

        public string GetSaveData()
        {
            var usableSlotsSaveData = new SaveDataItem[UsableSlots.Length];
            for (var i = 0; i < UsableSlots.Length; i++)
            {
                var slot = UsableSlots[i];
                usableSlotsSaveData[i] = new SaveDataItem()
                {
                    itemGuid = slot.ItemData ? slot.ItemData.Guid : string.Empty,
                    amount = slot.Quantity
                };
            }
            
            var bagSlotsSaveData = new SaveDataItem[BagSlots.Length];
            for (var i = 0; i < BagSlots.Length; i++)
            {
                var slot = BagSlots[i];
                bagSlotsSaveData[i] = new SaveDataItem()
                {
                    itemGuid = slot.ItemData ? slot.ItemData.Guid : string.Empty,
                    amount = slot.Quantity
                };
            }

            var saveData = new SaveData()
            {
                usableSlots = usableSlotsSaveData,
                bagSlots = bagSlotsSaveData,
            };
            
            return JsonUtility.ToJson(saveData);
        }

        public void LoadSaveData(in string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            
            var saveData = JsonUtility.FromJson<SaveData>(json);
            for (var i = 0; i < UsableSlots.Length; i++)
            {
                ref var saveDataItem = ref saveData.usableSlots[i];
                var item = DataManager.GetDataByGuid<UsableItemData>(saveDataItem.itemGuid);
                var amount = saveDataItem.amount;
                if (!item) continue;

                ref var slot = ref UsableSlots[i];
                slot = new UsableItemStack(item, amount);
                OnSlotChanged?.Invoke(SlotType.Usable, i);
            }

            for (var i = 0; i < BagSlots.Length; i++)
            {
                ref var saveDataItem = ref saveData.bagSlots[i];
                var item = DataManager.GetDataByGuid<UsableItemData>(saveDataItem.itemGuid);
                var amount = saveDataItem.amount;
                if (!item) continue;

                ref var slot = ref BagSlots[i];
                slot = new UsableItemStack(item, amount);
                OnSlotChanged?.Invoke(SlotType.Bag, i);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public SaveDataItem[] usableSlots;
            public SaveDataItem[] bagSlots;
        }

        [Serializable]
        private struct SaveDataItem
        {
            public string itemGuid;
            public int amount; 
        }
    }
}