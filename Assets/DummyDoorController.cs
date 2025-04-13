using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class DummyDoorController : MonoBehaviour
{
    public bool IsExit;
    public Image image;
    public GameObject ShopHelpText;
    public MMF_Player feedback;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            image.GetComponent<RectTransform>().DOScaleX(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(() => image.GetComponent<RectTransform>().DOScaleY(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(() => ShopHelpText.SetActive(true)));
            feedback.PlayFeedbacks();
            IsExit = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShopHelpText.SetActive(false);
            image.GetComponent<RectTransform>().DOScaleY(0.01f, 0.05f).SetEase(Ease.InOutExpo).OnComplete(() => image.GetComponent<RectTransform>().DOScaleX(0, 0.05f).SetEase(Ease.InOutExpo));
            feedback.PlayFeedbacks();
            IsExit = false;
        }
    }
}
