using System;
using UnityEngine;

namespace SubjectArena.Items
{
    public class GroundItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public UsableItemStack ItemStack;

        public event Action OnPickedUp;

        public void Setup(UsableItemStack itemStack)
        {
            
        }
        public void Pickup()
        {
            OnPickedUp?.Invoke();
            Destroy(gameObject);
        }
    }
}