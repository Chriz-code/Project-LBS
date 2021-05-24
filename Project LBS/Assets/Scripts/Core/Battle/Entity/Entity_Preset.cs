using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Entity_Preset", menuName = "ScriptableObjects/Entity Preset")]
public class Entity_Preset : ScriptableObject
{
    [System.Flags]
    public enum EntityFaction
    {
        None = 0,
        Neutral = 1,
        Player = 2,
        Enemy = 4,
        All = ~0,
    }
    [SerializeField] protected EntityFaction faction;
    public EntityFaction Faction
    {
        get => faction;
        set
        {
            if ((int)value == 6) faction = EntityFaction.None;
            else faction = value;
        }
    }
    public Sprite sprite = null;
    public int level = 1;
    public Stats stats = null;
    public Rank rank;

    protected virtual void OnValidate()
    {
        Faction = faction;
    }
}

