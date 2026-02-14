using SubjectArena.Items;
using UnityEngine;

namespace SubjectArena.Player
{
    public class PlayerItemDetector : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var groundItem = other.gameObject.GetComponentInParent<GroundItem>();
            if (!groundItem) return;

            if (_playerController.InventoryManager.CanAddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity))
            {
                _playerController.InventoryManager.AddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity);
                groundItem.Pickup();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}