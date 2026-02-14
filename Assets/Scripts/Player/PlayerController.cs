using System;
using SubjectArena.Input;
using SubjectArena.Movement;
using SubjectArena.Utils;
using UnityEngine;

namespace SubjectArena.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputProcessor playerInputProcessor;
        [SerializeField] private CharacterControllerMovement characterControllerMovement;

        private Camera _mainCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            ProcessPlayerMovement();
            //characterControllerMovement.LookAt(new Vector3(playerInputProcessor.MoveDirection.x, 0, playerInputProcessor.MoveDirection.y));
        }

        private void ProcessPlayerMovement()
        {
            var direction = GetCameraRelativeXZDirection(playerInputProcessor.MoveDirection);
            characterControllerMovement.Walk(direction);
            characterControllerMovement.LookAt(direction.ToVector3XZ());
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
            return camRelativeDirection.ToVector2X0Z();
        }
    }
}