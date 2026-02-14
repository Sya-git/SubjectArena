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

            if (_playerController.Inventory.CanAddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity))
            {
                _playerController.Inventory.AddItem(groundItem.ItemStack.ItemData, groundItem.ItemStack.Quantity);
                groundItem.Pickup();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}