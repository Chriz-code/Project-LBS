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
            battleActions.Add(Attack);
        }
        public void Attack(out float duration)
        {
            duration = 0.5f;
            if (string.IsNullOrEmpty(activeBook))
                return;

            if (BattleManager.inst.ability_Library.Books.TryGetValue(activeBook, out Ability_Book book) && targets != null)
                UseAbility(book, targets, out duration);
        }
        public void SetAbility(string ability)
        {
            activeBook = ability;
            stats.MP -= BattleManager.inst.ability_Library.Books[ability].ManaCost;
        }
        public void SetTargets(Entity_Character[] targets)
        {
            this.targets = targets;
        }
    }
}