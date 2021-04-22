using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSFX : MonoBehaviour
{
    [SerializeField] private AudioClip clip = null;
    [SerializeField]
    [Range(0f,1f)] private float volume = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = SoundHandler.instance.isSoundEffectMuted;
        SoundHandler.instance.AddSoundEffectSource(audioSource);
    }

    private void OnEnable()
    {
        /*if (audioSource != null)
            audioSource.mute = SoundHandler.instance.isSoundEffectMuted;*/
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
            Invoke("DisableObj", clip.length + .5f);
        }
        else
            Invoke("DisableObj", .5f);
    }

    private void DisableObj() => gameObject.SetActive(false);
}
