using SubjectArena.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SubjectArena.UI.Inventory
{
    public class CanvasInventoryManager : MonoBehaviour
    {
        [SerializeField] private CanvasItemSlot itemSlotPrefab;
        [SerializeField] private Transform usableItemsHolder;
        [SerializeField] private Transform bagItemsHolder;
        [SerializeField] private CanvasItemSlot dragItemDummy;

        private CanvasItemSlot[] _usableItems;
        private CanvasItemSlot[] _bagItems;

        private PlayerInventoryManager _inventoryManager;
        private bool _isDragging;

        private void Awake()
        {
            ClearAllCurrentItems();
        }

        private void OnDestroy()
        {
            if (_inventoryManager) _inventoryManager.OnSlotChanged -= OnInventorySlotChanged;
        }

        private void ClearAllCurrentItems()
        {
            var usableItems = usableItemsHolder.GetComponentsInChildren<CanvasItemSlot>();
            for (var i = 0; i < usableItems.Length; ++i)
            {
                Destroy(usableItems[i].gameObject);
            }
            
            var bagItems = bagItemsHolder.GetComponentsInChildren<CanvasItemSlot>();
            for (var i=0; i<bagItems.Length; ++i)
            {
                Destroy(bagItems[i].gameObject);
            }
            
            _bagItems = _usableItems = null;
        }

        public void Initialize(PlayerController player)
        {
            dragItemDummy.gameObject.SetActive(false);
            
            _inventoryManager = player.GetComponent<PlayerInventoryManager>();
            if (_inventoryManager)
            {
                _bagItems = new CanvasItemSlot[_inventoryManager.BagSlots.Length];
                for (var i = 0; i < _inventoryManager.BagSlots.Length; ++i)
                {
                    var bagSlot = _inventoryManager.BagSlots[i];
                    var canvasBagSlot = SpawnItemSlot(bagItemsHolder);
                    canvasBagSlot.Evt_OnBeginDrag += OnDragStart;
                    canvasBagSlot.Evt_OnDrag += OnDrag;
                    canvasBagSlot.Evt_OnEndDrag += OnDragEnd;
                    canvasBagSlot.Refresh(i, bagSlot, PlayerInventoryManager.SlotType.Bag);
                    _bagItems[i] = canvasBagSlot;
                }
                
                _usableItems = new CanvasItemSlot[_inventoryManager.UsableSlots.Length];
                for (var i=0; i<_inventoryManager.UsableSlots.Length; ++i)
                {
                    var usableSlot = _inventoryManager.UsableSlots[i];
                    var canvasUsableSlot = SpawnItemSlot(usableItemsHolder);
                    canvasUsableSlot.Evt_OnBeginDrag += OnDragStart;
                    canvasUsableSlot.Evt_OnDrag += OnDrag;
                    canvasUsableSlot.Evt_OnEndDrag += OnDragEnd;
                    canvasUsableSlot.Refresh(i, usableSlot, PlayerInventoryManager.SlotType.Usable);
                    _usableItems[i] = canvasUsableSlot;
                }
                _inventoryManager.OnSlotChanged += OnInventorySlotChanged;
            }
        }

        private void OnInventorySlotChanged(PlayerInventoryManager.SlotType type, int slotIndex)
        {
            if (!_inventoryManager.GetItemAtSlot(type, slotIndex, out var itemStack))
            {
                return;
            }
            
            var canvasItemSlot = (type == PlayerInventoryManager.SlotType.Usable
                ? _usableItems
                : _bagItems)[slotIndex];
            
            canvasItemSlot.Refresh(slotIndex, itemStack, type);
        }

        private void OnDragStart(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            if (_isDragging) return;
            
            dragItemDummy.Refresh(itemSlot.SlotIndex, itemSlot.ItemStack, itemSlot.SlotType);
            dragItemDummy.transform.position = pointerEvent.position;
            dragItemDummy.gameObject.SetActive(true);
            _isDragging = true;
        }

        private void OnDrag(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            dragItemDummy.transform.position = pointerEvent.position;
        }

        private void OnDragEnd(CanvasItemSlot fromSlot, PointerEventData pointerEvent)
        {
            dragItemDummy.gameObject.SetActive(false);

            if (pointerEvent.pointerCurrentRaycast.gameObject)
            {
                var toSlot = pointerEvent.pointerCurrentRaycast.gameObject.GetComponentInParent<CanvasItemSlot>();
                if (toSlot && _inventoryManager)
                {
                    _inventoryManager.SwapSlots(fromSlot.SlotType, toSlot.SlotType, fromSlot.SlotIndex, toSlot.SlotIndex);
                }
            }
            else
            {   // Trying to destroy item?
                _inventoryManager.DestroyItemAtSlot(fromSlot.SlotType, fromSlot.SlotIndex);
            }
            
            _isDragging = false;
        }

        private CanvasItemSlot SpawnItemSlot(Transform holder)
        {
            var slot = Instantiate(itemSlotPrefab, holder);
            return slot;
        }
    }
}