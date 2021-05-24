using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public delegate void StatChangeDel(string type, int val);

    #region Values
    [SerializeField] int health;
    public event StatChangeDel OnHealthChanged;
    public int HP
    {
        get => health;
        set
        {
            int old = health;
            health = value;
            if (old != value){
                OnHealthChanged.Invoke("Health", value - old);}
        }
    }

    [SerializeField] int mana;
    public event StatChangeDel OnManaChanged;
    public int MP
    {
        get => mana;
        set
        {
            int old = mana;
            mana = value;
            if (old != value) OnManaChanged.Invoke("Mana", value - old);
        }
    }

    [SerializeField] int attack;
    public event StatChangeDel OnAtkChanged;
    public int Atk
    {
        get => attack;
        set
        {
            int old = attack;
            attack = value;
            if (old != value) OnAtkChanged.Invoke("Attack", value - old);
        }
    }

    [SerializeField] int defence;
    public event StatChangeDel OnDefChanged;
    public int Def
    {
        get => defence;
        set
        {
            int old = defence;
            defence = value;
            if (old != value) OnDefChanged.Invoke("Defence", value - old);
        }
    }

    [SerializeField] int intelligence;
    public event StatChangeDel OnIntChanged;
    public int Int
    {
        get => intelligence;
        set
        {
            int old = intelligence;
            intelligence = value;
            if (old != value) OnIntChanged.Invoke("Intelligence", value - old);
        }
    }

    [SerializeField] int speed;
    public event StatChangeDel OnSpdChanged;
    public int Spd
    {
        get => speed;
        set
        {
            int old = speed;
            speed = value;
            if (old != value) OnSpdChanged.Invoke("Speed", value - old);
        }
    }

    [SerializeField] int magicPower;
    public event StatChangeDel OnMgpChanged;
    public int Mgp
    {
        get => magicPower;
        set
        {
            int old = magicPower;
            magicPower = value;
            if (old != value) OnMgpChanged.Invoke("Magic Power", value - old);
        }
    }
    #endregion
    public Stats(Stats stats)
    {
        health = stats.health;
        mana = stats.mana;
        attack = stats.attack;
        defence = stats.defence;
        intelligence = stats.intelligence;
        speed = stats.speed;
        magicPower = stats.magicPower;

        OnHealthChanged += StatChangeNotify;
        OnManaChanged += StatChangeNotify;
        OnAtkChanged += StatChangeNotify;
        OnDefChanged += StatChangeNotify;
        OnIntChanged += StatChangeNotify;
        OnSpdChanged += StatChangeNotify;
        OnMgpChanged += StatChangeNotify;
    }
    public Stats(int health, int mana, int attack, int defence, int intelligence, int speed, int magicPower)
    {
        this.health = health;
        this.mana = mana;
        this.attack = attack;
        this.defence = defence;
        this.intelligence = intelligence;
        this.speed = speed;
        this.magicPower = magicPower;

        OnHealthChanged += StatChangeNotify;
        OnManaChanged += StatChangeNotify;
        OnAtkChanged += StatChangeNotify;
        OnDefChanged += StatChangeNotify;
        OnIntChanged += StatChangeNotify;
        OnSpdChanged += StatChangeNotify;
        OnMgpChanged += StatChangeNotify;
    }
    public void StatChangeNotify(string type, int value)
    {
        Debug.Log($"{type}: {value}");
    }
    public void Apply(Stats stats)
    {
        health = stats.HP;
        mana = stats.MP;
    }
    public void Copy(Stats stats)
    {
        health = stats.health;
        mana = stats.mana;
        attack = stats.attack;
        defence = stats.defence;
        intelligence = stats.intelligence;
        speed = stats.speed;
        magicPower = stats.magicPower;
    }

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
            (
            a.health + b.health,
            a.mana + b.mana,
            a.attack + b.attack,
            a.defence + b.defence,
            a.intelligence + b.intelligence,
            a.speed + b.speed,
            a.magicPower + b.magicPower
            );
    }
}
