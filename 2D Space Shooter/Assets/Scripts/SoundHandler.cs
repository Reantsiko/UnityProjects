using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource audioSource = null;
    private int currentTrack = 0;
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

    private void Update()
    {
        if (/*!audioSource.isPlaying && */audioSource.time == audioSource.clip.length)
        {
            currentTrack++;
            if (currentTrack >= music.Length)
                    currentTrack = 0;
            audioSource.clip = music[currentTrack];
            audioSource.Play();
        }
    }
}
