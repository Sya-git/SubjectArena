using System;
using SubjectArena.Combat;
using UnityEngine;

namespace SubjectArena.Player
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        [SerializeField] private DamageArea damageArea;
        [SerializeField] private ParticleSystem attackStartVfx;
        [SerializeField] private float windupDuration = 0.3f;
        [SerializeField] private float attackDuration = 0.2f;
        [SerializeField] private float cooldownDuration = 0.5f;
        
        public enum AttackPhase : byte
        {
            Idle,
            Windup,
            Active,
            Cooldown
        }

        public event Action Evt_OnAttackStarted;

        private AttackPhase _phase;
        private float _timer;

        private void Start()
        {
            damageArea.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_phase == AttackPhase.Idle)
            {
                _phase = AttackPhase.Windup;
                _timer = 0f;
            }

            _timer += Time.deltaTime;

            switch (_phase)
            {
                case AttackPhase.Windup:
                    if (_timer >= windupDuration)
                        EnterActivePhase();
                    break;

                case AttackPhase.Active:
                    if (_timer >= attackDuration)
                        EnterCooldownPhase();
                    break;

                case AttackPhase.Cooldown:
                    if (_timer >= cooldownDuration)
                        EnterIdlePhase();
                    break;
            }
        }

        private void EnterActivePhase()
        {
            _phase = AttackPhase.Active;
            _timer = 0f;
            damageArea.gameObject.SetActive(true);
            attackStartVfx.Play();
            Evt_OnAttackStarted?.Invoke();
        }

        private void EnterCooldownPhase()
        {
            _phase = AttackPhase.Cooldown;
            _timer = 0f;
            damageArea.gameObject.SetActive(false);
        }

        private void EnterIdlePhase()
        {
            _phase = AttackPhase.Idle;
            _timer = 0f;
        }
    }
}