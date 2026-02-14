using System;
using System.Collections.Generic;
using System.Linq;
using SubjectArena.Entities;
using UnityEngine;

namespace SubjectArena.Combat
{
    [RequireComponent(typeof(AreaDetector))]
    public class DamagePerSecondArea : MonoBehaviour
    {
        [SerializeField] private uint damage = 1;

        private readonly Dictionary<Health, float> _targetsInArea = new();
        private readonly Dictionary<Health, float> _targetsCooldown = new();
        private AreaDetector _areaDetector;

        private void Awake()
        {
            _areaDetector = GetComponent<AreaDetector>();
            _areaDetector.OnTargetEnter += OnColliderDetected;
            _areaDetector.OnTargetExit += OnColliderLost;
        }

        private void AddTargetToList(Health health)
        {
            if (_targetsInArea.TryAdd(health, 0f))
            {
                if (_targetsCooldown.ContainsKey(health))
                {
                    var currentCooldown = _targetsCooldown[health];
                    _targetsInArea[health] = currentCooldown;
                    _targetsCooldown.Remove(health);
                }
                else
                {
                    health.TakeDamage(damage);
                }
            }
        }

        private void RemoveTargetFromList(Health health)
        {
            if (_targetsInArea.Remove(health, out var timer))
            {
                _targetsCooldown.Add(health, timer);
            }
        }
        
        private void OnColliderDetected(Collider other)
        {
            var health = other.GetComponentInParent<Health>();
            if (health) AddTargetToList(health);
        }

        private void OnColliderLost(Collider other)
        {
            var health = other.GetComponentInParent<Health>();
            if (health) RemoveTargetFromList(health);
        }

        private void Update()
        {
            for (var i = _targetsCooldown.Count - 1; i >= 0; i--)
            {
                var (health, timer) = _targetsCooldown.ElementAt(i);
                timer += Time.deltaTime;
                if (timer >= 1f)
                {
                    _targetsCooldown.Remove(health);
                }
                else
                {
                    _targetsCooldown[health] = timer;
                }
            }

            for (var i = _targetsInArea.Count - 1; i >= 0; i--)
            {
                var (health, timer) = _targetsInArea.ElementAt(i);
                timer += Time.deltaTime;
                if (timer >= 1f)
                {
                    health.TakeDamage(damage);
                    timer -= 1f;
                }
                
                _targetsInArea[health] = timer;
            }
        }
    }
}