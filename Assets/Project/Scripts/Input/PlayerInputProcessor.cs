using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SubjectArena.Input
{
    public class PlayerInputProcessor : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actionMap;
        public Vector2 MoveDirection { get; private set; }
        public bool UseItem { get; private set; }
        public int UsedItemIndex { get; private set; }
        
        private InputAction moveAction;
        private readonly List<InputAction> useItemActions = new();

        private void Awake()
        {
            moveAction = actionMap.FindAction("Move");
            for (var i = 0; i < 12; i++)
            {
                try
                {
                    var action = actionMap.FindAction($"UseItem{i}", true);
                    if (action != null)
                    {
                        useItemActions.Add(action);
                    }
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private void Update()
        {
            UseItem = false;
            for (var i = 0; i < useItemActions.Count; i++)
            {
                var action = useItemActions[i];
                if (action != null)
                {
                    UseItem = action.WasPressedThisFrame();
                    UsedItemIndex = i;
                }

                if (UseItem)
                {
                    break;
                }
            }

            if (moveAction != null)
            {
                MoveDirection = moveAction.ReadValue<Vector2>();
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (ctx.started || ctx.performed)
            {
                MoveDirection = ctx.ReadValue<Vector2>();
            }
            else
            {
                MoveDirection = Vector2.zero;
            }
        }

        public void UseItemOnSlot(InputAction.CallbackContext ctx, int slot)
        {
            if (ctx.started || ctx.performed)
            {
                UseItem = true;
                UsedItemIndex = 0;
            }
        }
    }
}