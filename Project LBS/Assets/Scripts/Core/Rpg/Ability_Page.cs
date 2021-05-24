using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability_Page
{   
    /// <summary>
    /// Damage or Healing
    /// </summary>
    public int strength = 1;
    /// <summary>
    /// Effect
    /// </summary>
    public StatusReference statusEffect;
}

[System.Serializable]
public class Status_Effect
{

}