using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player;
    public GameObject Target;

    [SerializeField] private double winScoreThreshold = 10000; // 승리 기준 점수

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        RhythmEvents.OnMusicStopped += OnMusicStopped;
    }

    private void OnDisable()
    {
        RhythmEvents.OnMusicStopped -= OnMusicStopped;
    }

    private void OnMusicStopped()
    {
        Debug.Log("음악 끝. 3초 뒤 결과 출력");
        StartCoroutine(HandleResultAfterDelay());
    }

    private IEnumerator HandleResultAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        double totalScore = ScoreManager.Instance.TotalScore;

        if (totalScore >= winScoreThreshold)
        {
            Debug.Log($"승리! 총 점수: {totalScore}");
        }
        else
        {
            Debug.Log($"패배 총 점수: {totalScore}");
        }
    }
}
