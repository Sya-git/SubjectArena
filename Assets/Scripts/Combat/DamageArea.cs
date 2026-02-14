using System;
using System.Collections.Generic;
using SubjectArena.Entities;
using UnityEngine;

namespace SubjectArena.Combat
{
    [RequireComponent(typeof(AreaDetector))]
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private uint damage = 1;
        

        private readonly HashSet<Health> _targetsInArea = new();
        private AreaDetector _areaDetector;

        private void Awake()
        {
            _areaDetector = GetComponent<AreaDetector>();
            _areaDetector.OnTargetEnter += OnColliderDetected;
        }

        private void OnEnable()
        {
            _targetsInArea.Clear();
        }

        private void AddTargetToList(Health health)
        {
            if (_targetsInArea.Add(health))
            {
                health.TakeDamage(damage);
            }
        }

        private void OnColliderDetected(Collider other)
        {
            var health = other.GetComponentInParent<Health>();
            if (health) AddTargetToList(health);
        }
    }
}