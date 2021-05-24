using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Values", menuName = "ScriptableObjects/Game Values")]
public class GameValues : ScriptableObject
{
    public int PlayerLevelAverage
    {
        get
        {
            int lvl = 0;
            foreach (var item in playerCharacters)
            {
                lvl += item.level;
            }
            return lvl / playerCharacters.Count;
        }
    }
    public List<Entity_Preset> playerCharacters = new List<Entity_Preset>();
}
