using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] private DifficultySettings diffSettings;

    private void Start()
    {
        if (!diffSettings)
            diffSettings = FindObjectOfType<DifficultySettings>();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(diffSettings.GetCurrentLevel());
    }

    public void LoadLevel(int index)
    {
        if (index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadSceneAsync(index); //change to async
        }
    }

    public void LoadNextLevel()
    {
        if (diffSettings.GetCurrentLevel() + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadSceneAsync(diffSettings.GetCurrentLevel() + 1); //change to async
        else
            Debug.Log("There are no more levels");
    }
}
