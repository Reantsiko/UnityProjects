using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource audioSource = null;
    private void Awake()
    {
        if (FindObjectsOfType<SoundHandler>().Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }

    public void PauseUnpauseMusic(bool pause)
    {
        if (pause)
            audioSource.Pause();
        else
            audioSource.UnPause();
    }

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /*private void Update()
    {
        if (Time.timeScale == 0 && audioSource.isPlaying)
            audioSource.Pause();
        else if (Time.timeScale == 1 && !audioSource.isPlaying)
            audioSource.UnPause();
    }*/
}
