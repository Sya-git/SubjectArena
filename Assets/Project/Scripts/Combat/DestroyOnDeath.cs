using System.Collections;
using UnityEngine;

namespace SubjectArena.Combat
{
    public class DestroyOnDeath : MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 1f;
        [SerializeField] private Health health;
        [SerializeField] private ParticleSystem destroyParticles;

        private void Awake()
        {
            health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            StartCoroutine(PlayParticlesAndDestroy());
        }

        private IEnumerator PlayParticlesAndDestroy()
        {
            yield return new WaitForSeconds(destroyDelay);
            
            if (destroyParticles)
            {
                destroyParticles.transform.SetParent(null, true);
                destroyParticles.Play();
                Destroy(destroyParticles.gameObject, destroyParticles.main.duration);
            }
            Destroy(gameObject);
        }
    }
}