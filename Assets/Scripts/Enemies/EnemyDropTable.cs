using System;
using System.Collections.Generic;
using SubjectArena.Entities;
using SubjectArena.Items;
using UnityEngine;

namespace SubjectArena.Enemies
{
    public class EnemyDropTable : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private List<DropEntry> possibleDrops = new ();

        [Serializable]
        public class DropEntry
        {
            public UsableItemData itemData;
            public int minQuantity = 1;
            public int maxQuantity = 1;
            
            public int oneInX = 100;

            [HideInInspector] public float Probability => 1f / oneInX;
        }

        private void OnEnable()
        {
            health.OnDeath += SpawnDropOnDeath;
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnDeath -= SpawnDropOnDeath;
        }

        private void SpawnDropOnDeath()
        {
            DropItems();
        }

        private void DropItems()
        {
            if (possibleDrops == null || possibleDrops.Count == 0)
                return;

            var spawnPosition = transform.position;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out var hitInfo, 10f, 1 << LayerMask.NameToLayer("Ground")))
            {
                spawnPosition = hitInfo.point;
            }
            
            foreach (var drop in possibleDrops)
            {
                var itemData = drop.itemData;
                if (!itemData)
                    continue;

                // Decide se esse item cai ou não
                if (UnityEngine.Random.value <= drop.Probability)
                {
                    var quantity = UnityEngine.Random.Range(drop.minQuantity, drop.maxQuantity + 1);

                    if (quantity <= 0)
                        continue;

                    var droppedItem = Instantiate(itemData.GroundItemPrefab, spawnPosition, Quaternion.identity);

                    droppedItem.Setup(new UsableItemStack(itemData, quantity));
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Evita valores inválidos
            foreach (var entry in possibleDrops)
            {
                if (entry.oneInX < 1) entry.oneInX = 1;
                if (entry.minQuantity < 1) entry.minQuantity = 1;
                if (entry.maxQuantity < entry.minQuantity) entry.maxQuantity = entry.minQuantity;
            }
        }
#endif
    }
}