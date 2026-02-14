using UnityEngine;

namespace SubjectArena.Combat
{
    public class DestroyOnDeath : MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 1f;
        [SerializeField] private Health health;

        private void Awake()
        {
            health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}