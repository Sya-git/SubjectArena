using System;
using System.Collections;
using SubjectArena.Enemies;
using SubjectArena.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SubjectArena.Managers
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField] private WaveSpawnDataArray[] waveSpawnData;
        [SerializeField] private float spawnRadius = 1f;
        [SerializeField] private ParticleSystem spawnParticles;
        [Min(1)]
        [SerializeField] private float waveSpawnDelay = 5f;

        private int _currentWave = 0;
        private float _waveSpawnTimer = 0f;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        private void Start()
        {
            _waveSpawnTimer = 0f;
        }

        private void Update()
        {
            _waveSpawnTimer += Time.deltaTime;
            if (_waveSpawnTimer >= waveSpawnDelay)
            {
                StartCoroutine(SpawnCurrentWave(_currentWave));
                _currentWave++;
                _waveSpawnTimer = 0;
            }
        }

        private IEnumerator SpawnCurrentWave(int currentWave)
        {
            if (currentWave >= waveSpawnData.Length)
            {
                currentWave = 0;
            }
            
            var currentWaveData = waveSpawnData[currentWave];
            foreach (var spawn in currentWaveData.spawnData)
            {
                if (!spawn.enemyPrefab) continue;
                
                for (var i=0; i<spawn.amount; i++)
                {
                    var spawnPos = transform.position + Random.insideUnitCircle.ToVector3X0Z() * spawnRadius;
                    StartCoroutine(SpawnEnemy(spawn.enemyPrefab, spawnPos));
                    yield return null;
                }
            }
        }

        private IEnumerator SpawnEnemy(EnemyController enemy, Vector3 pos)
        {
            Instantiate(enemy, pos, Quaternion.identity);
            if (spawnParticles)
            {
                var vfx = Instantiate(spawnParticles, pos, Quaternion.identity);
                vfx.Play();
                
                yield return new WaitForSeconds(vfx.main.duration);
                
                Destroy(vfx.gameObject);
            }
        }
        
    }

    [Serializable]
    public struct WaveSpawnDataArray
    {
        public WaveSpawn[] spawnData;
    }

    [Serializable]
    public struct WaveSpawn
    {
        public EnemyController enemyPrefab;
        public int amount;
    }
}