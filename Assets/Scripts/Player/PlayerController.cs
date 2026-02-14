using System;
using SubjectArena.Entities;
using SubjectArena.Input;
using SubjectArena.Movement;
using SubjectArena.Utils;
using UnityEngine;

namespace SubjectArena.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputProcessor playerInputProcessor;
        [SerializeField] private CharacterControllerMotor characterControllerMotor;
        [SerializeField] private PlayerInventoryManager inventoryManager;
        [SerializeField] private Health health;

        public PlayerInventoryManager InventoryManager => inventoryManager;
        private Camera _mainCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            characterControllerMotor.Walk(Vector2.zero);
        }

        private void Update()
        {
            if (!health.IsAlive) return;
            
            ProcessPlayerMovement();
            CheckForPlayerUsedItems();
        }

        private void ProcessPlayerMovement()
        {
            var direction = GetCameraRelativeXZDirection(playerInputProcessor.MoveDirection);
            characterControllerMotor.Walk(direction);
            characterControllerMotor.LookAt(direction.ToVector3X0Z());
        }

        private void CheckForPlayerUsedItems()
        {
            if (playerInputProcessor.UseItem)
            {
                inventoryManager.UseItemAtSlot(playerInputProcessor.UsedItemIndex);
            }
        }
        
        private Vector2 GetCameraRelativeXZDirection(Vector2 direction)
        {
            if (!_mainCamera || direction.sqrMagnitude < 0.01f)
            {
                return Vector2.zero;
            }

            var camForward = _mainCamera.transform.forward;
            var camRight = _mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
            var camRelativeDirection = (camForward * direction.y + camRight * direction.x).normalized;
            return camRelativeDirection.ToVector2XZ();
        }
    }
}