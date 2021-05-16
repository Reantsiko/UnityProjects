using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cube;
    public float timeToClick = 5f;
    public static CubeSpawner instance;
    public Slider timeSlider;
    public Text text;
    public Text restartText;
    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Time.timeScale = 1;
        timeSlider.maxValue = timeToClick;
        timeSlider.value = timeToClick;
        text.text = score.ToString();
        Instantiate(cube);
    }

    public void SpawnNewCube()
    {
        Instantiate(cube);
        score++;
        text.text = score.ToString();
        if (timeToClick > 1f)
            timeToClick -= 0.01f;
        timeSlider.maxValue = timeToClick;
        timeSlider.value = timeToClick;
    }

    private void Update()
    {
        timeSlider.value -= Time.deltaTime;
        if (Time.timeScale != 0 && timeSlider.value <= 0f)
        {
            Time.timeScale = 0;
            StartCoroutine(Restart());
        }
    }

    private IEnumerator Restart()
    {
        restartText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(0);
    }
}
