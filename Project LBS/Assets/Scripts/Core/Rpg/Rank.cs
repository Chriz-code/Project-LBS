using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rank
{
    public string name = "";
    public Stats stats = new Stats(1, 1, 1, 1, 1, 1, 1);
    public List<BookReference> books = new List<BookReference>();
}
