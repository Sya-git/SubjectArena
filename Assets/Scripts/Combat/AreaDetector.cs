using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectArena.Combat
{
    public class AreaDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        
        public event Action<Collider> OnTargetEnter;
        public event Action<Collider> OnTargetExit;
        
        private readonly HashSet<Collider> collidersInArea = new();

        private void AddColliderToList(Collider col)
        {
            if (collidersInArea.Add(col))
            {
                OnTargetEnter?.Invoke(col);
            }
        }

        private void RemoveColliderFromList(Collider col)
        {
            if (!collidersInArea.Contains(col)) return;
            
            collidersInArea.Remove(col);
            OnTargetExit?.Invoke(col);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || (targetLayer & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            AddColliderToList(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger || (targetLayer & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }
            
            RemoveColliderFromList(other);
        }
    }
}