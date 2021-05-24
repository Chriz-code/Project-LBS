using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum AbilityType
{
    None = 0,
    Passive = 1,
    Active = 2,
    Support = 4,
    Reactive = 8,
    Unique = 16,
    Everything = ~0,
}

[System.Flags]
public enum TargetType
{
    None = 0,
    Single = 1,
    Multi = 2,
    Random = 4,
    All = ~0,
}

[System.Serializable]
public class Ability_Book
{
    public string Key = "";
    public int Index = 0;
    public int ManaCost = 1;
    public AbilityType AbilityType = AbilityType.None;
    public TargetType TargetType = TargetType.Single;
    public int TargetCount = 1;
    public Ability_Page[] Pages = new Ability_Page[1];
}

[System.Serializable]
public class BookReference : ISerializationCallbackReceiver
{
    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList;
    [ListToPopup(typeof(BookReference), "TMPList")]
    [UnityEngine.Serialization.FormerlySerializedAs("Value")]
    public string Value = string.Empty;
    public List<string> GetAllBookInBuild()
    {
        List<string> allBooks = new List<string>();
        Ability_Library libraryData = (Ability_Library)Resources.Load<ScriptableObject>("Ability Library");
        if (!libraryData)
            Debug.LogWarning("MISSING LIBRARY");

        foreach (var item in libraryData.books)
        {
            allBooks.Add(item.Key);
        }

        return allBooks;
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        PopupList = GetAllBookInBuild();
        if (PopupList.Count <= 0)
        {
            Debug.LogWarning("Book List Empty!");
            return;
        }
        TMPList = PopupList;
        if (Value == string.Empty)
            Value = PopupList[0];
    }
}

[System.Serializable]
public class Status_Book
{
    public string Key = "";
    public Status_Effect Value;
}

[System.Serializable]
public class StatusReference : ISerializationCallbackReceiver
{
    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList;
    [ListToPopup(typeof(StatusReference), "TMPList")]
    [UnityEngine.Serialization.FormerlySerializedAs("Value")]
    public string Value = string.Empty;
    public List<string> GetAllStatusesInBuild()
    {
        List<string> allBooks = new List<string>();
        Ability_Library libraryData = (Ability_Library)Resources.Load<ScriptableObject>("Ability Library");
        if (!libraryData)
            Debug.LogWarning("MISSING LIBRARY");

        foreach (var item in libraryData.statusEffects)
        {
            allBooks.Add(item.Key);
        }

        return allBooks;
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        PopupList = GetAllStatusesInBuild();
        if (PopupList.Count <= 0)
        {
            Debug.LogWarning("Status List Empty!");
            return;
        }
        TMPList = PopupList;
        if (Value == string.Empty)
            Value = PopupList[0];
    }
}