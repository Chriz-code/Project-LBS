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
        public List<Status_Effect> statModifiers = new List<Status_Effect>();
        public Stats MaxStats { get; protected set; }
        public Stats StatsMod
        {
            get
            {
                Stats s = new Stats(Stats);
                foreach (var item in statModifiers) s += item.statChange;
                return s;
            }
        }
        [ReadOnly] public Stats Stats;
        [ReadOnly, TextArea, SerializeField] Stats statsMod;

        public Entity_Preset.EntityFaction Faction { get => preset.Faction; }
        public virtual void Initiate(Entity_Preset preset)
        {
            onDeath.AddListener(BattleManager.inst.RemoveEntity);
            this.preset = preset;
            GetComponent<SpriteRenderer>().sprite = preset.sprite;
            Stats = new Stats(preset.stats);
            MaxStats = new Stats(Stats);
            BattleManager.inst.GetTurnSystem.onPassiveAfter.Add(StatusTurn);
            BattleManager.inst.GetTurnSystem.onTurnChange.AddListener(InfoUpdate);
            name = preset.name;
        }
        public virtual void Initiate(Entity_Preset preset, string name)
        {
            Initiate(preset);
            this.name = name;
        }
        protected abstract void Awake();
        protected abstract void Start();

        public void StatusTurn(out float duration)
        {
            duration = 0.1f;
            for (int i = 0; i < statModifiers.Count; i++)
            {
                if (statModifiers[i].statusType.HasFlag(StatusType.Temporary))
                {
                    statModifiers[i].turnsOfEffect--;
                    if (statModifiers[i].turnsOfEffect <= 0)
                    {
                        statModifiers.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    statModifiers[i].turnsOfEffect--;
                    Stats += statModifiers[i].statChange;
                    if (statModifiers[i].turnsOfEffect <= 0)
                    {
                        statModifiers.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        public void Damage(int val)
        {
            Stats.HP -= Mathf.Max(val - StatsMod.Def, 0);
            if (Stats.HP <= 0)
            {
                Debug.Log($"{name} just fkn died!");
                onDeath.Invoke(this);
            }
        }
        public void Heal(int val)
        {
            if (Stats.HP > 0)
            {
                Stats.HP += val;
            }
        }
        public void Effect(Status_Effect statusEffect)
        {
            statModifiers.Add(new Status_Effect(statusEffect));
        }

        private void InfoUpdate(TurnSystem.Turns turn)
        {
            statsMod = StatsMod;
        }
    }
}
