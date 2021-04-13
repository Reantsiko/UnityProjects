using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] music = null;
    [SerializeField] private AudioSource audioSource = null;
    private void Awake() => DontDestroyOnLoad(gameObject);

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
}
