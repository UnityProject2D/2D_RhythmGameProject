using System;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    public static RestartManager Instance { get; private set; }
    public event Action OnRestartGame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void RestartFromFirstStage()
    {
        try
        {
            OnRestartGame?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during RestartGame: {e.Message}");
        }
    }
}
