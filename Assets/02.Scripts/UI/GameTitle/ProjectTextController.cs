using FMODUnity;
using UnityEngine;

public class ProjectTextController : MonoBehaviour
{
    public void PlaySFX()
    {
        RuntimeManager.PlayOneShot("event:/SFX/KeyboardSound");
    }
}
