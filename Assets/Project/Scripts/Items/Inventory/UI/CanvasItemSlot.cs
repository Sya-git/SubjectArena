using System;
using SubjectArena.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SubjectArena.Items.Inventory.UI
{
    public class CanvasItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
        IPointerMoveHandler, IPointerExitHandler
    {
        [FormerlySerializedAs("_imgItem")]
        [SerializeField] private Image imgItem;
        [FormerlySerializedAs("_txtAmount")]
        [SerializeField] private TMP_Text txtAmount;
        [SerializeField] private TMP_Text txtUseInput;

        public UsableItemStack ItemStack { get; private set; }
        public InventoryController.SlotType SlotType { get; private set; }
        public int SlotIndex { get; private set; }

        public event Action<CanvasItemSlot, PointerEventData> Evt_OnBeginDrag;
        public event Action<CanvasItemSlot, PointerEventData> Evt_OnDrag;
        public event Action<CanvasItemSlot, PointerEventData> Evt_OnEndDrag;
        public event Action<CanvasItemSlot, PointerEventData> Evt_OnPointerEnter;
        public event Action<CanvasItemSlot, PointerEventData> Evt_OnPointerMove;
        public event Action<CanvasItemSlot, PointerEventData> Evt_OnPointerExit;


        public void OnBeginDrag(PointerEventData eventData) => Evt_OnBeginDrag?.Invoke(this, eventData);

        public void OnDrag(PointerEventData eventData) => Evt_OnDrag?.Invoke(this, eventData);

        public void OnEndDrag(PointerEventData eventData) => Evt_OnEndDrag?.Invoke(this, eventData);
        
        public void Refresh(int slotIndex, UsableItemStack itemStack, InventoryController.SlotType slotType)
        {
            SlotIndex = slotIndex;
            SlotType = slotType;
            ItemStack = itemStack;

            if (itemStack.ItemData)
            {
                imgItem.sprite = itemStack.ItemData.Icon;
                imgItem.color = Color.white;
                txtAmount.text = $"x{itemStack.Quantity}";

                imgItem.color = itemStack.Quantity == 0
                    ? new Color(imgItem.color.r, imgItem.color.g, imgItem.color.b, 0.5f)
                    : Color.white;
            }
            else
            {
                imgItem.sprite = null;
                imgItem.color = new Color(1, 1, 1, 0);
                txtAmount.text = "";
            }

            if (slotType == InventoryController.SlotType.Usable)
            {
                txtUseInput.text = (SlotIndex + 1).ToString();
            }
            else
            {
                txtUseInput.gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) => Evt_OnPointerEnter?.Invoke(this, eventData);
        public void OnPointerMove(PointerEventData eventData) => Evt_OnPointerMove?.Invoke(this, eventData);
        public void OnPointerExit(PointerEventData eventData) => Evt_OnPointerExit?.Invoke(this, eventData);
    }
}