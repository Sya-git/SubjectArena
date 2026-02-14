using UnityEngine;
using UnityEngine.InputSystem;

namespace SubjectArena.Input
{
    public class PlayerInputProcessor : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement;
        
        public Vector2 MoveDirection { get; private set; }

        private void Update()
        {
            MoveDirection = movement ? movement.action.ReadValue<Vector2>() : Vector2.zero;
        }
    }
}