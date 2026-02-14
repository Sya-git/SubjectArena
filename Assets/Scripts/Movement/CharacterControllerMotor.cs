using UnityEngine;

namespace SubjectArena.Movement
{
    public class CharacterControllerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 720f;

        private Vector3 _inputDirection;
        private Vector3 _lookDirection;
        private float _verticalVelocity;
        private Camera _mainCamera;

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();
        }

        public void Walk(Vector2 direction)
        {
            _inputDirection = new Vector3(direction.x, 0f, direction.y).normalized;
        }

        public void LookAt(Vector3 worldDirection)
        {
            worldDirection.y = 0f;
            if (worldDirection.sqrMagnitude > 0.01f)
            {
                var normalizedDirection = worldDirection.normalized;
                _lookDirection = new Vector3(normalizedDirection.z, 0, -normalizedDirection.x);
            }
        }

        private void ApplyMovement()
        {
            if (characterController.isGrounded)
            {
                _verticalVelocity = -0.5f;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            var velocity = _inputDirection * moveSpeed;
            velocity.y = _verticalVelocity;

            characterController.Move(velocity * Time.deltaTime);
        }

        private void ApplyRotation()
        {
            if (_lookDirection.sqrMagnitude < 0.01f)
                return;

            var targetRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        
    }
}