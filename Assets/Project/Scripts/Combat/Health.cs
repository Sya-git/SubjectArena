using System;
using UnityEngine;

namespace SubjectArena.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private uint baseHealth = 10;
        
        public uint MaxHealth => baseHealth;
        public uint CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;
        public event Action OnDeath;
        public delegate void OnHealthChangedDelegate(uint oldValue, uint newValue);
        public event OnHealthChangedDelegate Evt_OnHealthChanged;

        private void Awake()
        {
            CurrentHealth = baseHealth;
        }

        public void TakeDamage(uint damage)
        {
            if (CurrentHealth <= 0) return;
            
            var oldHealth = CurrentHealth;
            CurrentHealth -= Math.Min(damage, CurrentHealth);
            if (oldHealth != CurrentHealth)
            {
                Evt_OnHealthChanged?.Invoke(oldHealth, CurrentHealth);
            }
            
            if (CurrentHealth == 0) OnDeath?.Invoke();
        }

        public void Heal(uint amount)
        {
            if (CurrentHealth <= 0) return;

            var oldHealth = CurrentHealth;
            CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
            if (oldHealth != CurrentHealth)
            {
                Evt_OnHealthChanged?.Invoke(oldHealth, CurrentHealth);
            }
        }
    }
}