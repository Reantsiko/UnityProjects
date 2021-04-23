using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SoundHandler : MonoBehaviour
{
    public static SoundHandler instance;
    [Header("Music Related")]
    public bool isMusicMuted;
    [Range(0f,1f)] public float musicVolume = 1f;
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource musicSource = null;
    [SerializeField] private string musicKey = null;
    [Header("SFX Related")]
    public bool isSoundEffectMuted;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [SerializeField] private List<AudioSource> soundEffectSources = new List<AudioSource>();
    [SerializeField] private string sfxKey = null;

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

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        //musicVolume = LoadVolumes(musicKey);
        sfxVolume = LoadVolumes(sfxKey);
        ChangeMusicVolume(LoadVolumes(musicKey));
        ChangeSFXVolume(sfxVolume);
    }

    public void ChangeMuteMusic() => musicSource.mute = !musicSource.mute;
    public void ChangeMuteSoundEffect() => soundEffectSources.ForEach(sfx => sfx.mute = isSoundEffectMuted);
    public void ClearSoundEffectSources() => soundEffectSources.Clear();
    public void ChangeMusicVolume(float val)
    {
        if (musicSource != null)
            musicSource.volume = val;//musicVolume;
    }

    public void ChangeSFXVolume(float val = 1f)
    {
        sfxVolume = val;
        soundEffectSources.ForEach(sfx => sfx.volume = sfxVolume);
    }

    private void Update()
    {
        if (GameManager.instance.menu != null && !GameManager.instance.menu.isPaused && !musicSource.isPlaying)
        {
            currentTrack++;
            if (currentTrack >= music.Length)
                currentTrack = 0;
            musicSource.clip = music[currentTrack];
            musicSource.Play();
        }
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
    private float LoadVolumes(string key)
    {
        if (key == null || !PlayerPrefs.HasKey(key)) return 1f;
        return PlayerPrefs.GetFloat(key);
    }
}
