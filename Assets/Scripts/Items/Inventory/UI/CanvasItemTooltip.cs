using TMPro;
using UnityEngine;

namespace SubjectArena.Items.Inventory.UI
{
    public class CanvasItemTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;

        public UsableItemStack ItemStack { get; private set; }
        public InventoryController.SlotType SlotType { get; private set; }
        public int SlotIndex { get; private set; }
        
        public void Activate(UsableItemStack item, InventoryController.SlotType slotType, int slotIndex)
        {
            ItemStack = item;
            SlotType = slotType;
            SlotIndex = slotIndex;
            
            if (item.ItemData)
            {
                itemName.text = item.ItemData.ItemName;
                itemDescription.text = item.ItemData.Description;
                
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Move(Vector3 position)
        {
            transform.position = position;
        }

        public void Disable()
        {
            itemName.text = "";
            itemDescription.text = "";
            
            gameObject.SetActive(false);
        }
    }
}