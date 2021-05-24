using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    abstract public class Entity : MonoBehaviour
    {
        public UnityEvent<Entity> onDeath = new UnityEvent<Entity>();

        [SerializeField] protected Entity_Preset preset = null;
        public Stats maxStats;
        public Stats stats;

        public Entity_Preset.EntityFaction Faction { get => preset.Faction; }

        public virtual void Initiate(Entity_Preset preset)
        {
            onDeath.AddListener(BattleManager.inst.RemoveEntity);
            this.preset = preset;
            GetComponent<SpriteRenderer>().sprite = preset.sprite;
            stats = new Stats(preset.stats);
            maxStats = new Stats(stats);
            name = preset.name;
        }
        public virtual void Initiate(Entity_Preset preset, string name)
        {
            Initiate(preset);
            this.name = name;
        }
        protected abstract void Awake();
        protected abstract void Start();
        public void Damage(int val)
        {
            stats.HP -= val;
            if (stats.HP <= 0)
            {
                Debug.Log($"{name} just fkn died!");
                onDeath.Invoke(this);
            }
        }
    }
}
