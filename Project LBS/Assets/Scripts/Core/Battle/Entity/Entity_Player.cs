using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Entity_Player : Entity_Character
    {
        public Ability_Book activeBook = null;
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
            if (activeBook == null)
                return;

            if (targets != null)
            {
                UseAbility(activeBook, targets, out duration);
            }
        }
        public void SetAbility(Ability_Book book)
        {
            activeBook = book;
        }
        public void SetTargets(Entity_Character[] targets)
        {
            SelectFlash(targets, Color.black);
            this.targets = targets;
            Stats.MP -= activeBook.ManaCost;
        }
        public void SetTarget(Entity_Character target)
        {
            this.targets = new Entity_Character[] { target };
        }
    }
}