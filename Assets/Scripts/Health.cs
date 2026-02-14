using UnityEngine;

namespace SubjectArena.Entities
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private uint baseHealth = 10;
        
        public uint MaxHealth => baseHealth;
        public uint CurrentHealth { get; private set; }
        
        public void TakeDamage(uint damage)
        {
            CurrentHealth -= damage;
        }
    }
}