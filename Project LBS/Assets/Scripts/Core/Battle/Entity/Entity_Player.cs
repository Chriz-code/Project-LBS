using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Entity_Player : Entity_Character
    {
        public string activeBook = string.Empty;
        Entity_Character[] targets;
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            battleActions.Add(PlayerAttack);
        }
        public void PlayerAttack(out float duration)
        {
            duration = 0.2f;
            if (string.IsNullOrEmpty(activeBook))
                return;

            if (BattleManager.inst.ability_Library.Books.TryGetValue(activeBook, out Ability_Book book) && targets != null)
            {
                UseAbility(book, targets, out duration);
            }
        }
        public void SetAbility(string ability)
        {
            activeBook = ability;
            Stats.MP -= BattleManager.inst.ability_Library.Books[ability].ManaCost;
        }
        public void SetTargets(Entity_Character[] targets)
        {
            SelectFlash(targets, Color.black);
            this.targets = targets;
        }
        public void SetTarget(Entity_Character target)
        {
            this.targets = new Entity_Character[] { target };
        }
    }
}