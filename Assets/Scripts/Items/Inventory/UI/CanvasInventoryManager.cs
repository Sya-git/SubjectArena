using SubjectArena.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SubjectArena.Items.Inventory.UI
{
    public class CanvasInventoryManager : MonoBehaviour
    {
        [SerializeField] private CanvasItemSlot itemSlotPrefab;
        [SerializeField] private Transform usableItemsHolder;
        [SerializeField] private Transform bagItemsHolder;
        [SerializeField] private CanvasItemSlot dragItemDummy;
        [SerializeField] private CanvasItemTooltip itemTooltip;

        private CanvasItemSlot[] _usableItems;
        private CanvasItemSlot[] _bagItems;

        private InventoryController _inventory;
        private bool _isDragging;

        private void Awake()
        {
            ClearAllCurrentItems();
        }

        private void OnDestroy()
        {
            if (_inventory) _inventory.OnSlotChanged -= OnInventorySlotChanged;
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
            itemTooltip.gameObject.SetActive(false);
            
            _inventory = player.GetComponent<InventoryController>();
            if (_inventory)
            {
                _bagItems = new CanvasItemSlot[_inventory.BagSlots.Length];
                for (var i = 0; i < _inventory.BagSlots.Length; ++i)
                {
                    var bagSlot = _inventory.BagSlots[i];
                    var canvasBagSlot = SpawnItemSlot(bagItemsHolder);
                    canvasBagSlot.Evt_OnBeginDrag += OnDragStart;
                    canvasBagSlot.Evt_OnDrag += OnDrag;
                    canvasBagSlot.Evt_OnEndDrag += OnDragEnd;
                    canvasBagSlot.Evt_OnPointerEnter += OnPointerEnter;
                    canvasBagSlot.Evt_OnPointerMove += OnPointerMove;
                    canvasBagSlot.Evt_OnPointerExit += OnPointerExit;
                    canvasBagSlot.Refresh(i, bagSlot, InventoryController.SlotType.Bag);
                    _bagItems[i] = canvasBagSlot;
                }
                
                _usableItems = new CanvasItemSlot[_inventory.UsableSlots.Length];
                for (var i=0; i<_inventory.UsableSlots.Length; ++i)
                {
                    var usableSlot = _inventory.UsableSlots[i];
                    var canvasUsableSlot = SpawnItemSlot(usableItemsHolder);
                    canvasUsableSlot.Evt_OnBeginDrag += OnDragStart;
                    canvasUsableSlot.Evt_OnDrag += OnDrag;
                    canvasUsableSlot.Evt_OnEndDrag += OnDragEnd;
                    canvasUsableSlot.Evt_OnPointerEnter += OnPointerEnter;
                    canvasUsableSlot.Evt_OnPointerMove += OnPointerMove;
                    canvasUsableSlot.Evt_OnPointerExit += OnPointerExit;
                    canvasUsableSlot.Refresh(i, usableSlot, InventoryController.SlotType.Usable);
                    _usableItems[i] = canvasUsableSlot;
                }
                _inventory.OnSlotChanged += OnInventorySlotChanged;
            }
        }

        private void OnInventorySlotChanged(InventoryController.SlotType type, int slotIndex)
        {
            if (!_inventory.GetItemAtSlot(type, slotIndex, out var itemStack))
            {
                return;
            }
            
            var canvasItemSlot = (type == InventoryController.SlotType.Usable
                ? _usableItems
                : _bagItems)[slotIndex];
            
            canvasItemSlot.Refresh(slotIndex, itemStack, type);
            if (itemTooltip.gameObject.activeSelf && itemTooltip.SlotType == type && itemTooltip.SlotIndex == slotIndex)
            {
                itemTooltip.Activate(canvasItemSlot.ItemStack, type, slotIndex);
            }
        }

        private void OnDragStart(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            if (_isDragging) return;
            
            dragItemDummy.Refresh(itemSlot.SlotIndex, itemSlot.ItemStack, itemSlot.SlotType);
            dragItemDummy.transform.position = pointerEvent.position;
            dragItemDummy.gameObject.SetActive(true);
            SetDragging(true);
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
                if (toSlot && _inventory)
                {
                    _inventory.SwapSlots(fromSlot.SlotType, toSlot.SlotType, fromSlot.SlotIndex, toSlot.SlotIndex);
                }
            }
            else
            {   // Trying to destroy item?
                _inventory.DestroyItemAtSlot(fromSlot.SlotType, fromSlot.SlotIndex);
            }
            
            SetDragging(false);
        }
        
        private void OnPointerEnter(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            if (_isDragging) return;

            if (itemSlot.ItemStack.ItemData)
            {
                itemTooltip.Activate(itemSlot.ItemStack, itemSlot.SlotType, itemSlot.SlotIndex);
            }
        }

        private void OnPointerMove(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            if (_isDragging) return;
            
            if (itemTooltip.gameObject.activeSelf)
            {
                itemTooltip.Move(pointerEvent.position);
            }
            else
            {
                itemTooltip.Activate(itemSlot.ItemStack, itemSlot.SlotType, itemSlot.SlotIndex);
            }
        }

        private void OnPointerExit(CanvasItemSlot itemSlot, PointerEventData pointerEvent)
        {
            if (_isDragging) return;
            
            if (itemTooltip.gameObject.activeSelf)
            {
                itemTooltip.Disable();
            }
        }

        private void SetDragging(bool isDragging)
        {
            _isDragging = isDragging;

            if (_isDragging && itemTooltip.gameObject.activeSelf)
            {
                itemTooltip.Disable();
            }
        }

        private CanvasItemSlot SpawnItemSlot(Transform holder)
        {
            var slot = Instantiate(itemSlotPrefab, holder);
            return slot;
        }
    }
}