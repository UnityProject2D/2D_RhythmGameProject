using System;
using UnityEngine;
public class SFXSound : MonoBehaviour
{
    public AudioClip SfxClip;
    public float Volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Play()
    {
        if (SfxClip != null)
            audioSource.PlayOneShot(SfxClip, Volume);
    }

}
