using UnityEngine;

public class ShopMusicPlayer : MonoBehaviour
{
    [SerializeField] private TitleMusicPlayer titleMusicPlayer;

    private void Start()
    {
        if (titleMusicPlayer != null)
        {
            titleMusicPlayer.Play();
        }
    }
}