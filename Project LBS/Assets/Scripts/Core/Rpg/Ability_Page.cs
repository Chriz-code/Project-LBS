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

[System.Flags]
public enum StatusType { None = 0, Temporary = 1}
[System.Serializable]
public class Status_Effect
{
    public int turnsOfEffect = 0;
    public StatusType statusType = StatusType.Temporary;
    /// <summary>
    /// Debuff or Buff
    /// </summary>
    public Stats statChange;

    public Status_Effect(Status_Effect s)
    {
        turnsOfEffect = s.turnsOfEffect;
        statusType = s.statusType;
        statChange = s.statChange;
    }
}