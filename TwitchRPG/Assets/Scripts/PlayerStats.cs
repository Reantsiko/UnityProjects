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

    public int currentExperience;
    public int experienceForNextLevel;
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

        experienceForNextLevel = (int)(500 * level * 1.5);
    }
    public PlayerStats(){}

    public void GainXP(int amountGained)
    {
        currentExperience += amountGained;
        if (currentExperience > experienceForNextLevel)
        {
            level++;
            var remainder = currentExperience - experienceForNextLevel;
            experienceForNextLevel = (int)(500 * level * 1.5);
            currentExperience = remainder;
        }
    }
}
