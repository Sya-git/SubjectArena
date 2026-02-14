using System;
using System.Collections.Generic;
using SubjectArena.Combat;
using SubjectArena.Input;
using SubjectArena.Movement;
using SubjectArena.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace SubjectArena.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputProcessor playerInputProcessor;
        [SerializeField] private CharacterControllerMotor characterControllerMotor;
        [FormerlySerializedAs("inventoryManager")] [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Health health;

        public PlayerInventory Inventory => inventory;
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
                inventory.UseItemAtSlot(playerInputProcessor.UsedItemIndex);
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

        public string GetSaveData()
        {
            var inventorySaveData = inventory.GetSaveData();
            var saveData = new PlayerSaveData()
            {
                Inventory = inventorySaveData
            };
            
            return JsonUtility.ToJson(saveData);
        }

        public void LoadSaveData(in string json)
        {
            var saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            inventory.LoadSaveData(saveData.Inventory);
        }

        [Serializable]
        private struct PlayerSaveData
        {
            public string Inventory;
        }
    }
}