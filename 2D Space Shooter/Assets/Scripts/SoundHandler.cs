using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundHandler : MonoBehaviour
{
    public static SoundHandler instance;
    public bool isMusicMuted;
    public bool isSoundEffectMuted;
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource musicSource = null;
    [SerializeField] private List<AudioSource> soundEffectSources = new List<AudioSource>();
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
            musicSource.Pause();
        else
            musicSource.UnPause();
    }

    private void Start() => musicSource = GetComponent<AudioSource>();

    public void ChangeMuteMusic() => musicSource.mute = !musicSource.mute;
    public void ChangeMuteSoundEffect() => soundEffectSources.ForEach(sfx => sfx.mute = isSoundEffectMuted);
    public void ClearSoundEffectSources() => soundEffectSources.Clear();

    private void Update()
    {
#if UNITY_STANDALONE
        if (musicSource.time == musicSource.clip.length)
        {
            currentTrack++;
            if (currentTrack >= music.Length)
                    currentTrack = 0;
            musicSource.clip = music[currentTrack];
            musicSource.Play();
        }
#endif
#if UNITY_WEBGL
        if (!musicSource.isPlaying)
        {
            currentTrack++;
            if (currentTrack >= music.Length)
                currentTrack = 0;
            musicSource.clip = music[currentTrack];
            musicSource.Play();
        }
#endif
    }

    public void SetSound(bool val)
    {
        if (musicSource == null) return;
        musicSource.mute = val;
    }

    public void AddSoundEffectSource(AudioSource toAdd) => soundEffectSources.Add(toAdd);


    public void SetSoundEffect(bool val)
    {
        var sources = FindObjectsOfType<AudioSource>().ToList();
        
        sources.ForEach(s => s.mute = val);
    }

    public bool GetIsMuted() => musicSource.mute;
}
