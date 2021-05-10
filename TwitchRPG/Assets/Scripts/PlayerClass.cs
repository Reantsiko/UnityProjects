using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class PlayerClassLevel
{
    public int level;
    public int experience;

    public PlayerClassLevel(int lvl, int xp)
    {
        level = lvl;
        experience = xp;
    }
}
[System.Serializable]
public class PlayerClass
{
    public Dictionary<PClass, PlayerClassLevel> playerClass;
    public PClass activeClass;
    private int experienceForNextLevel;
    public PlayerClass(PClass selectedClass)
    {
        playerClass = new Dictionary<PClass, PlayerClassLevel>();
        var temp = System.Enum.GetValues(typeof(PClass)).Length;
        for (int i = 0; i < temp; i++)
            playerClass.Add((PClass)i, new PlayerClassLevel(1, 0));
        activeClass = selectedClass;
        CalculateExperienceForLevel();
    }

    public void GainXP(int amountGained)
    {
        playerClass[activeClass].experience += amountGained;
        if (playerClass[activeClass].experience >= experienceForNextLevel)
        {
            playerClass[activeClass].level++;
            var remainder = playerClass[activeClass].experience - experienceForNextLevel;
            CalculateExperienceForLevel();
            playerClass[activeClass].experience = remainder;
        }
        Debug.Log($"Active Class: {activeClass}, Level: {playerClass[activeClass].level}, Current Experience: {playerClass[activeClass].experience}, Experience till level: {experienceForNextLevel - playerClass[activeClass].experience}");
    }

    public void ChangePlayerClass(PClass toSet)
    {
        activeClass = toSet;
        CalculateExperienceForLevel();
    }

    private void CalculateExperienceForLevel() => experienceForNextLevel = (int)(500 * playerClass[activeClass].level * 1.5);
}