using SubjectArena.Items.Inventory;
using UnityEngine;

namespace SubjectArena.Items
{
    public class GroundItemGatherer : MonoBehaviour
    {
        private InventoryController _inventory;

        private void Awake()
        {
            _inventory = GetComponentInParent<InventoryController>();
        }

        private void TryGatherGroundItem(GroundItem groundItem)
        {
            if (!groundItem) return;
            if (!_inventory.CanAddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity)) return;
            
            _inventory.AddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity);
            groundItem.Pickup();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryGatherGroundItem(other.gameObject.GetComponentInParent<GroundItem>());
        }
    }
}