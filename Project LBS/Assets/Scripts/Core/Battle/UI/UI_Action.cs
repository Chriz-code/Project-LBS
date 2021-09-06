using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Battle
{
    public class UI_Action : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<Entity_Player, Ability_Book> onAbilitySet = new UnityEvent<Entity_Player, Ability_Book>();
        [HideInInspector] public UnityEvent<Entity_Character[]> onTargetSet = new UnityEvent<Entity_Character[]>();

        [HideInInspector] public Ability_Book book;
        public Entity_Preset.EntityFaction targetFaction = Entity_Preset.EntityFaction.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="bookIndex">Relative Value to Player</param>
        public void SetAbility(int bookIndex)
        {
            Entity_Player player = BattleManager.inst.IndexPlayer;
            book = BattleManager.inst.ability_Library.Books[player.rank.books[bookIndex].Value];
            Debug.Log($"{player.name} uses {book.Key}");
            targetFaction = book.TargetType == TargetType.Support ? Entity_Preset.EntityFaction.Player : Entity_Preset.EntityFaction.Enemy;
            player.SetAbility(book);
            onAbilitySet.Invoke(player, book);
        }

        [SerializeField] List<Entity_Character> targets = new List<Entity_Character>();

        public void SetTarget(int yVal)
        {
            if (targetFaction == Entity_Preset.EntityFaction.None)
            {
                Debug.Log("Missing faction");
                return;
            }
            if (book.TargetType.HasFlag(TargetType.Random))
            {
                targets.AddRange(BattleManager.inst.GetFaction(targetFaction));
                TargetsSet();
            }
            else
            {
                targets.Add(BattleManager.inst.GetFaction(targetFaction)[yVal]);
                if (targets.Count == book.TargetCount)
                    TargetsSet();
            }
        }
        void TargetsSet()
        {
            BattleManager.inst.IndexPlayer.SetTargets(targets.ToArray());
            onTargetSet.Invoke(targets.ToArray());
        }
        public void Reset()
        {
            book = null;
            targetFaction = Entity_Preset.EntityFaction.None;
            onAbilitySet.Invoke(BattleManager.inst.IndexPlayer, null);
            targets = new List<Entity_Character>();
        }
    }
}
