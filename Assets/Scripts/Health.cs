using System;
using UnityEngine;

namespace SubjectArena.Entities
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private uint baseHealth = 10;
        
        public uint MaxHealth => baseHealth;
        public uint CurrentHealth { get; private set; }

        private void Start()
        {
            CurrentHealth = baseHealth;
        }

        public void TakeDamage(uint damage)
        {
            if (CurrentHealth <= 0) return;
            
            CurrentHealth -= Math.Min(damage, CurrentHealth);
        }
    }
}