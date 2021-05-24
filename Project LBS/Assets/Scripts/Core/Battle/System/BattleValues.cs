using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Is set outside of battle to determine type of battle in scene
    /// </summary>
    [CreateAssetMenu(fileName = "Battle Values", menuName = "ScriptableObjects/Battle Values")]
    public class BattleValues : ScriptableObject
    {
        public BattleTemplate template = BattleTemplate.Test;

        public List<Entity_Preset> additionalEnemies = new List<Entity_Preset>();
    }
}