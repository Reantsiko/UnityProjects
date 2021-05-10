using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerJobLevel
{
    public int level;
    public int experience;

    public PlayerJobLevel(int lvl, int xp)
    {
        level = lvl;
        experience = xp;
    }
}
[System.Serializable]
public class PlayerJob
{
    public Dictionary<PJob, PlayerJobLevel> playerJob;
    public PJob activeJob;
    private int experienceForNextLevel;
    public PlayerJob(PJob selectedJob)
    {
        playerJob = new Dictionary<PJob, PlayerJobLevel>();
        var temp = System.Enum.GetValues(typeof(PJob)).Length;
        for (int i = 0; i < temp; i++)
            playerJob.Add((PJob)i, new PlayerJobLevel(0, 0));
        activeJob = selectedJob;
    }

    public void GainXP(int amountGained)
    {
        playerJob[activeJob].experience += amountGained;
        if (playerJob[activeJob].experience >= experienceForNextLevel)
        {
            playerJob[activeJob].level++;
            var remainder = playerJob[activeJob].experience - experienceForNextLevel;
            CalculateExperienceForLevel();
            playerJob[activeJob].experience = remainder;
        }
        Debug.Log($"Active Class: {activeJob}, Level: {playerJob[activeJob].level}, Current Experience: {playerJob[activeJob].experience}, Experience till level: {experienceForNextLevel - playerJob[activeJob].experience}");
    }

    public void SetPlayerJob(PJob toSet)
    {
        activeJob = toSet;
        CalculateExperienceForLevel();
    }

    private void CalculateExperienceForLevel() => experienceForNextLevel = (int)(1500 * playerJob[activeJob].level * 1.5);
}