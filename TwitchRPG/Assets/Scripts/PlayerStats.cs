using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerStats
{
    public int level;
    public int strength;
    public int dexterity;
    public int constitution;
    public int charisma;
    public int intelligence;

    public int maxHitPoints;
    public int maxMagicPoints;

    public int hp;
    public int mp;
    public PlayerStats(int lvl, int str, int dex, int cst, int chr, int intel)
    {
        level = lvl;
        strength = str;
        dexterity = dex;
        constitution = cst;
        charisma = chr;
        intelligence = intel;

        maxHitPoints = constitution / 2;
        maxMagicPoints = intelligence + (charisma / 2);

        hp = maxHitPoints;
        mp = maxMagicPoints;
    }
    public PlayerStats(){}
}
