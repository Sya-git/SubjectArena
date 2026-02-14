using SubjectArena.Entities;
using UnityEngine;

namespace SubjectArena.Items
{
    [CreateAssetMenu(menuName = "SubjectArena/Items/Heal Potion")]
    public class HealItemData : UsableItemData
    {
        [SerializeField] private uint healAmount = 5;

        public override bool Use(GameObject user)
        {
            var health = user.GetComponent<Health>();
            if (!health || !health.IsAlive)
                return false;

            if (health.CurrentHealth >= health.MaxHealth)
                return false;

            health.Heal(healAmount);
            return true;
        }
    }
}