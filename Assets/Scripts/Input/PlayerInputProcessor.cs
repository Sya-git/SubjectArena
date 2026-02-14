using UnityEngine;
using UnityEngine.InputSystem;

namespace SubjectArena.Input
{
    public class PlayerInputProcessor : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement;
        [SerializeField] private InputActionReference[] useItem;
        
        public Vector2 MoveDirection { get; private set; }
        public bool UseItem { get; private set; }
        public int UsedItemIndex { get; private set; }

        private void Update()
        {
            UseItem = false;
            MoveDirection = movement ? movement.action.ReadValue<Vector2>() : Vector2.zero;
            for (var i = 0; i < useItem.Length; ++i)
            {
                UseItem |= useItem[i].action.WasPressedThisFrame();
                if (UseItem)
                {
                    UsedItemIndex = i;
                    break;
                }
            }
        }
    }
}