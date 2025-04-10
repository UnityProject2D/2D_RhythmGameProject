using System.Collections;
using FMODUnity;
using UnityEngine;

public class GameTitleController : MonoBehaviour
{
    [SerializeField] private float _bgmDelay = 1.5f;

    private void Start()
    {
        StartCoroutine(PlayBGMWithDelay(_bgmDelay));
    }

    IEnumerator PlayBGMWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBGM();
    }

    public void PlayBGM()
    {
        RuntimeManager.PlayOneShot("event:/Title");
    }
}
