using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Entity_Enemy : Entity_Character
    {
        AI_Computer computer = null;
        public string activeBook = "";
        public Entity_Character[] targets = null;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            computer = BattleManager.inst.GetAI;
            BattleManager.inst.GetTurnSystem.onEnemy.Add(Think);
            BattleManager.inst.GetTurnSystem.onEnemy.Add(ShowTarget);
            battleActions.Add(Attack);
        }

        public void Think(out float duration)
        {
            duration = 0.4f;
            string move = computer.CalculateMove(this, out Ability_Book book);
            if (stats.MP < book.ManaCost)
            {
                Debug.Log(name + " Out of Mana!");
                return;
            }
            if (string.IsNullOrEmpty(move))
            {
                Debug.Log("AI TOO DUMB SELF DESTRUCT");
                Destroy(gameObject);
            }
            activeBook = move;
            if (book.TargetType.HasFlag(TargetType.Random))
                BattleManager.inst.TryGetRandoms(book.TargetCount, Entity_Preset.EntityFaction.Player, out targets);
            stats.MP -= book.ManaCost;
        }

        public void ShowTarget(out float duration)
        {
            duration = 0.4f;
            foreach (var item in targets)
            {
                item.Flash(Color.black, 0.2f);
            }
        }

        public void Attack(out float duration)
        {
            duration = 0.5f;
            if (BattleManager.inst.ability_Library.Books.TryGetValue(activeBook, out Ability_Book book))
                UseAbility(book, targets, out duration);
        }
    }
}
