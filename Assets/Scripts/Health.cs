using System;
using UnityEngine;

namespace SubjectArena.Entities
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private uint baseHealth = 10;
        
        public uint MaxHealth => baseHealth;
        public uint CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;
        public event Action OnDeath;

        private void Start()
        {
            CurrentHealth = baseHealth;
        }

        public void TakeDamage(uint damage)
        {
            if (CurrentHealth <= 0) return;
            
            CurrentHealth -= Math.Min(damage, CurrentHealth);
            if (CurrentHealth == 0) OnDeath?.Invoke();
        }

        public void Heal(uint amount)
        {
            if (CurrentHealth <= 0) return;

            CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
        }
    }
}