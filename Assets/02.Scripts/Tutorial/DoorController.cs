using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;
using static RhythmEvents;

public class DoorController : MonoBehaviour
{
    private Animator _animator;
    public GameObject Light;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        _animator.SetTrigger("Open");
        Light.SetActive(true);

        RuntimeManager.PlayOneShot("event:/SFX/DoorOpen");
    }
}
