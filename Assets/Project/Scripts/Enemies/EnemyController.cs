using SubjectArena.Combat;
using SubjectArena.Managers;
using SubjectArena.Movement;
using SubjectArena.Player;
using SubjectArena.Utils;
using UnityEngine;

namespace SubjectArena.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        private static readonly int AnimMovingParameter = Animator.StringToHash("Moving");
        private static readonly int AnimDieParameter = Animator.StringToHash("Die");
        private static readonly int AnimGetHitParameter = Animator.StringToHash("Hit");
        
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterControllerMotor motor;
        [SerializeField] private Health health;

        private void Start()
        {
            health.OnDeath += OnDeath;
            health.Evt_OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(uint oldValue, uint newValue)
        {
            if (newValue != 0)
            {
                animator.SetTrigger(AnimGetHitParameter);
            }
        }

        private void OnDeath()
        {
            motor.Walk(Vector2.zero);
            animator.SetTrigger(AnimDieParameter);
        }

        private void Update()
        {
            if (!health.IsAlive)
            {
                animator.SetBool(AnimMovingParameter, false);
                return;
            }
            
            var gameManager = GameManager.Instance;
            if (gameManager && gameManager.Player)
            {
                var moveDirection = GetMoveDirectionToPlayer(gameManager.Player);
                motor.Walk(moveDirection);
                motor.LookAt(moveDirection.ToVector3X0Z());
                animator.SetBool(AnimMovingParameter, true);
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