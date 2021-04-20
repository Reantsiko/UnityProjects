using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundHandler : MonoBehaviour
{
    public static SoundHandler instance;
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource audioSource = null;
    private int currentTrack = 0;
    private void Awake()
    {
        if (FindObjectsOfType<SoundHandler>().Length == 1)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
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

    private void Start() => audioSource = GetComponent<AudioSource>();

    private void Update()
    {
        if (audioSource.time == audioSource.clip.length)
        {
            currentTrack++;
            if (currentTrack >= music.Length)
                    currentTrack = 0;
            audioSource.clip = music[currentTrack];
            audioSource.Play();
        }
    }

    public void SetSound(bool val)
    {
        if (audioSource == null) return;

        audioSource.mute = val;
        var sources = FindObjectsOfType<AudioSource>().ToList();
        sources.ForEach(s => s.mute = val);
    }

    public bool GetIsMuted() => audioSource.mute;
}
