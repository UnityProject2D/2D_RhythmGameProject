using MoreMountains.Feedbacks;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    [SerializeField] private MMF_Player _feedbackPlayer;

    private MMF_TMPTextReveal _textReveal;

    private void Awake()
    {
        _textReveal = _feedbackPlayer.GetFeedbackOfType<MMF_TMPTextReveal>();
        if (_textReveal == null)
        {
            Debug.LogError("MMF_TMPTextReveal component not found on this GameObject.");
        }
    }
}
