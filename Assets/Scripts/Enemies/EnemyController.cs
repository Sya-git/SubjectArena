using SubjectArena.Movement;
using SubjectArena.Player;
using SubjectArena.Utils;
using UnityEngine;

namespace SubjectArena.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private CharacterControllerMotor motor;
        
        private void Update()
        {
            var gameManager = GameManager.Instance;
            if (gameManager && gameManager.Player)
            {
                var moveDirection = GetMoveDirectionToPlayer(gameManager.Player);
                motor.Walk(moveDirection);
                motor.LookAt(moveDirection.ToVector3X0Z());
            }
        }

        private Vector2 GetMoveDirectionToPlayer(PlayerController player)
        {
            if (!player)
            {
                return Vector2.zero;
            }
            
            var directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0f;
            directionToPlayer.Normalize();
            return directionToPlayer.ToVector2XZ();
        }
    }
}