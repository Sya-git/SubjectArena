using SubjectArena.Combat;
using SubjectArena.Movement;
using SubjectArena.Player;
using SubjectArena.Utils;
using UnityEngine;

namespace SubjectArena.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private CharacterControllerMotor motor;
        [SerializeField] private Health health;

        private void Start()
        {
            health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            motor.Walk(Vector2.zero);
        }

        private void Update()
        {
            if (!health.IsAlive) return;
            
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