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
    //public Dictionary<PJob, PlayerJobLevel> playerJob;
    public PJob job;
    public PlayerJobLevel jobLevel;
    private int experienceForNextLevel;
    public PlayerJob(PJob selectedJob)
    {
        job = selectedJob;
        jobLevel = new PlayerJobLevel(1, 0);
        CalculateExperienceForLevel();
    }

    public void GainXP(int amountGained)
    {
        jobLevel.experience += amountGained;
        if (jobLevel.experience >= experienceForNextLevel)
        {
            jobLevel.level++;
            var remainder = jobLevel.experience - experienceForNextLevel;
            CalculateExperienceForLevel();
            jobLevel.experience = remainder;
        }
    }

    private void CalculateExperienceForLevel() => experienceForNextLevel = (int)(1500 * jobLevel.level * 1.5);
}