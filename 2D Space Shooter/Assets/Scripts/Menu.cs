using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus = null;
    [SerializeField] private GameObject quitButton = null;
    [SerializeField]private int sceneBuildIndex = 0;
    [SerializeField] private bool isPaused = false;
    public void StartGame() => SceneManager.LoadScene(1);
    public void OpenMenu(GameObject toOpen)
    {
        foreach (var menu in menus)
            menu.SetActive(false);
        toOpen.SetActive(true);
    }
    public void QuitGame() => Application.Quit();

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
