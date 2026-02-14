using System;
using SubjectArena.Items;
using SubjectArena.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Image _imgItem;
    [SerializeField] private TMP_Text _txtAmount;
    
    public UsableItemStack ItemStack { get; private set; }
    public PlayerInventory.SlotType SlotType { get; private set; }
    public int SlotIndex { get; private set; }

    public event Action<CanvasItemSlot, PointerEventData> Evt_OnBeginDrag;
    public event Action<CanvasItemSlot, PointerEventData> Evt_OnDrag;
    public event Action<CanvasItemSlot, PointerEventData> Evt_OnEndDrag;
    

    public void OnBeginDrag(PointerEventData eventData) => Evt_OnBeginDrag?.Invoke(this, eventData);
    
    public void OnDrag(PointerEventData eventData) => Evt_OnDrag?.Invoke(this, eventData);

    public void OnEndDrag(PointerEventData eventData) => Evt_OnEndDrag?.Invoke(this, eventData);

    public void Refresh(int slotIndex, UsableItemStack itemStack, PlayerInventory.SlotType slotType)
    {
        SlotIndex = slotIndex;
        SlotType = slotType;
        ItemStack = itemStack;
        
        if (itemStack.ItemData)
        {
            _imgItem.sprite = itemStack.ItemData.Icon;
            _txtAmount.text = itemStack.Quantity.ToString();

            _imgItem.color = itemStack.Quantity == 0
                ? new Color(_imgItem.color.r, _imgItem.color.g, _imgItem.color.b, 0.5f)
                : Color.white;
        }
        else
        {
            _imgItem.sprite = null;
            _txtAmount.text = "";
        }
    }
}
