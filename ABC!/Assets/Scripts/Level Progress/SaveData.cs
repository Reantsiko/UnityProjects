using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    private int[] levelProgress;
    private Dictionary<Difficulty, float[]> bestTimesDifficulty;
    private Dictionary<Difficulty, float[]> bestMessesDifficulty;

    public void InputLoaded(SaveData data)
    {
        LevelProgress = data.LevelProgress;
        BestTimesDifficulty = data.BestTimesDifficulty;
        BestMessesDifficulty = data.BestMessesDifficulty;
    }

    public Dictionary<Difficulty, float[]> BestMessesDifficulty
    {
        get { return bestMessesDifficulty; }
        set { bestMessesDifficulty = value; }
    }

    public Dictionary<Difficulty, float[]> BestTimesDifficulty
    {
        get { return bestTimesDifficulty; }
        set { bestTimesDifficulty = value; }
    }

    public int[] LevelProgress
    {
        get { return levelProgress; }
        set { levelProgress = value; }
    }

    public void LevelCompleted(int level)
    {
        levelProgress[level] = 1;
    }
    public int GetLevelCompleted(int level) { return levelProgress[level]; }
}
