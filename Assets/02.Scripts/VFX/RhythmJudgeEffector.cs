using UnityEngine;

public class RhythmJudgeEffector : MonoBehaviour
{
    private void Start()
    {
        RhythmEvents.OnInputJudged += OnRhythmJudge;
    }

    private void OnRhythmJudge(JudgedContext judge)
    {
        if (judge.Result == JudgementResult.Perfect)
        {
            Debug.Log("Perfect Effect");
            VFXManager.Instance.PlayOnPerfectFeedback();
        }
        else if (judge.Result == JudgementResult.Good)
        {
            Debug.Log("Good Effect");

            VFXManager.Instance.PlayOnGoodFeedback();
        }
        else if (judge.Result == JudgementResult.Bad)
        {
            Debug.Log("Bad Effect");
        }
        else if (judge.Result == JudgementResult.Miss)
        {
            Debug.Log("Miss Effect");
        }
    }
}
