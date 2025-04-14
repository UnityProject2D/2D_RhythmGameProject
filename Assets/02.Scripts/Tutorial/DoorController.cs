using FMODUnity;
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator _animator;
    public GameObject Light;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        RhythmEvents.OnMusicStopped += ValidateOpen;
        RhythmEvents.OnMarkerHit += ValidateOpen;
        StartCoroutine(OpenDoorWhenShop());
    }

    private void OnDestroy()
    {
        RhythmEvents.OnMusicStopped -= ValidateOpen;
        RhythmEvents.OnMarkerHit -= ValidateOpen;
    }
    IEnumerator OpenDoorWhenShop()
    {
        yield return new WaitForSeconds(3);
        Debug.Log(GameSceneManager.Instance.CurrentStage);
        if(GameSceneManager.Instance.CurrentStage < 0)
        {
            Open();
        }
    }
    private void ValidateOpen()
    {
        if (GameSceneManager.Instance.CurrentStage != 0)
        {
            Open();
        }
    }
    private void ValidateOpen(string a)
    {

        if (a == "End" && GameSceneManager.Instance.CurrentStage != 0)
        {
            Open();
        }
    }
    public void Open()
    {
        _animator.SetTrigger("Open");
        Light.SetActive(true);

        RuntimeManager.PlayOneShot("event:/SFX/DoorOpen");
    }
}
