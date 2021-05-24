using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [RequireComponent(typeof(BattleManager))]
    public class AI_Computer : MonoBehaviour
    {
        public string CalculateMove(Entity_Enemy entity, out Ability_Book book)
        {
            //Use ai in the future to get the most optimal ability
            if (BattleManager.inst.ability_Library.Books.TryGetValue(entity.rank.books[Random.Range(0, entity.rank.books.Count)].Value, out book))
                return book.Key;
            return string.Empty;
        }
    }
}
