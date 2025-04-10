using System.Collections;
using FMODUnity;
using UnityEngine;

public class GameTitleController : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private TitleMusicPlayer _titleMusicPlayer;
    [SerializeField] private float _bgmDelay = 1.5f;

    private void Start()
    {
        StartCoroutine(PlayBGMWithDelay(_bgmDelay));
    }

    IEnumerator PlayBGMWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _titleMusicPlayer.Play();
    }
}
