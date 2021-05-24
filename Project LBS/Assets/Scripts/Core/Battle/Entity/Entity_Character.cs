using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Entity_Character : Entity
    {
        public Rank rank;
        public List<TurnSystem.TurnEvent> battleActions = new List<TurnSystem.TurnEvent>();
        SpriteRenderer sr = null;
        protected override void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        protected override void Start()
        {
            BattleManager.inst.GetTurnSystem.onPassiveAfter.Add(ManaTurnHeal);
            stats.OnHealthChanged += DamageFlash;
        }
        public override void Initiate(Entity_Preset preset)
        {
            base.Initiate(preset);
            rank = preset.rank;
            stats = new Stats(preset.stats) + rank.stats;
            maxStats = new Stats(stats);
            name = preset.name;
        }
        public override void Initiate(Entity_Preset preset, string name)
        {
            Initiate(preset);
            this.name = name;
        }

        public void ManaTurnHeal(out float duration)
        {
            duration = 0.1f;
            stats.MP = Mathf.Min(stats.MP + 1, maxStats.MP);
        }
        public void ApplyStatusEffect(Status_Effect effect)
        {

        }

        /// <summary>
        /// Selected Targets
        /// </summary>
        /// <param name="book"></param>
        /// <param name="targets"></param>
        protected void UseAbility(Ability_Book book, Entity_Character[] targets, out float duration)
        {
            duration = 1f;
            Debug.Log($"{name} just used: {book.Key}!");
            foreach (var item in book.Pages)
            {
                if (book.AbilityType.HasFlag(AbilityType.Active))
                {
                    if (book.AbilityType.HasFlag(AbilityType.Unique))
                    {
                        BattleManager.inst.ability_Library.UniqueBooks[book.Key].Invoke
                            (
                            new CustomAbilityInfo() { targets = targets },
                            out CustomAbilityInfo outfo,
                            out duration
                            );
                        if (outfo.abilityType.HasFlag(CustomAbilityInfo.CustomAbilityType.PrintText))
                        {
                            Debug.Log(outfo.sValue);
                        }
                    }
                    else
                    {
                        foreach (var target in targets)
                        {
                            target.Damage(item.strength);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Temp
        /// </summary>
        /// <param name="val"></param>
        protected void DamageFlash(string type, int val)
        {
            if (val < 0)
                Flash(Color.red, 0.1f);
            else if (val > 0)
                Flash(Color.green, 0.1f);
        }
        public void Flash(Color color, float time)
        {
            colorList.Add(color);
            if (flashing == false)
            {
                StartCoroutine(FlashColor(time));
            }
        }

        public void FlashRepeat(Color color, float time)
        {
            if (repeat == false)
            {
                repeat = true;
                StartCoroutine(FlashColorRepeat(color, time));
            }
        }
        public void FlashRepeatStop()
        {
            repeat = false;
        }


        /// <summary>
        /// Only 2D Spire Object
        /// </summary>
        /// <param name="color"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool flashing = false;
        List<Color> colorList = new List<Color>();
        protected IEnumerator FlashColor(float time)
        {
            flashing = true;
            Color col = sr.color;
            for (int i = 0; i < colorList.Count; i++)
            {
                sr.color = colorList[i];
                yield return new WaitForSeconds(time);
                colorList.RemoveAt(i);
                sr.color = col;
                i--;
                yield return new WaitForSeconds(time);
            }
            flashing = false;
        }

        bool repeat = false;
        protected IEnumerator FlashColorRepeat(Color color, float time)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color col = sr.color;

            while (repeat)
            {
                sr.color = color;
                yield return new WaitForSeconds(time);
                sr.color = col;
            }
        }
    }
}