using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus = null;
    [SerializeField] private GameObject quitButton = null;
    [SerializeField] private int sceneBuildIndex = 0;
    [SerializeField] private bool isPaused = false;
    [SerializeField] private Image soundImage = null;
    [SerializeField] private Sprite[] soundSprites = null;
    [SerializeField] private AudioSource audioSource = null;
    public void StartGame(int toSet)
    {
        GameManager.instance.ResetSettings((Difficulty)toSet);
        SceneManager.LoadSceneAsync(1);
    }
    
    public void OpenMenu(GameObject toOpen)
    {
        foreach (var menu in menus)
            menu.SetActive(false);
        toOpen.SetActive(true);
    }
    public void QuitGame() => Application.Quit();

    public void SetSound()
    {
        bool val = !audioSource.mute;
        int img = val ? 1 : 0;
        if (soundImage != null)
            soundImage.sprite = soundSprites[img];
        audioSource.mute = val;
    }

    private void Start()
    {
        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
#if UNITY_WEBGL
        if (quitButton != null)
            quitButton.SetActive(false);
#endif
    }

    private void Update()
    {
        if (sceneBuildIndex == 0) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }
    }
}
