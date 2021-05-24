using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Ability_Library", menuName = "ScriptableObjects/Ability Library")]
public class Ability_Library : ScriptableObject
{
    public delegate void Unique(CustomAbilityInfo info, out CustomAbilityInfo outfo, out float duration);
    public List<Ability_Book> books = new List<Ability_Book>();
    public List<Status_Book> statusEffects = new List<Status_Book>();

    public Dictionary<string, Ability_Book> Books = null;
    public Dictionary<string, Status_Effect> StatusEffects = null;
    public Dictionary<string, Unique> UniqueBooks = null;

    public void Initialize()
    {
        //Hash'em
        Books = new Dictionary<string, Ability_Book>();
        StatusEffects = new Dictionary<string, Status_Effect>();
        UniqueBooks = new Dictionary<string, Unique>();
        int i = 0;
        foreach (var item in books)
        {
            item.Index = i;
            Books.Add(item.Key, item);
            i++;
        }
        foreach (var item in statusEffects)
        {
            StatusEffects.Add(item.Key, item.Value);
        }

        UniqueBooks.Add("Identify", Identify);
    }

    private void OnValidate()
    {
        for (int i = 0; i < books.Count; i++)
        {
            books[i].Index = i;
        }
    }

    /// <summary>
    /// books has to contain Ability Book with identical function name as key and have AbilityType set to Unique
    /// </summary>
    /// <param name="info"></param>
    /// <param name="outfo"></param>
    /// <param name="duration"></param>
    #region UniqueAbilities
    public void Identify(CustomAbilityInfo info, out CustomAbilityInfo outfo, out float duration)
    {
        duration = .2f;
        outfo = new CustomAbilityInfo();
        foreach (var item in info.targets)
        {
            foreach (var ability in item.rank.books)
            {
                outfo.sValue += ability.Value + ", ";
            }
        }
        outfo.abilityType = CustomAbilityInfo.CustomAbilityType.PrintText;
    }
    #endregion
}

[System.Serializable]
public struct CustomAbilityInfo
{
    [System.Flags]
    public enum CustomAbilityType { None = 0, All = ~0, PrintText = 1 }
    public Battle.Entity_Character[] targets;
    public CustomAbilityType abilityType;

    public string sValue;
}
