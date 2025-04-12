using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class ShopHelper : MonoBehaviour
{
    public Collider2D shopCollider;
    public Image image;
    public GameObject ShopHelpText;
    public MMF_Player feedback;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            image.GetComponent<RectTransform>().DOScaleX(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(()=>image.GetComponent<RectTransform>().DOScaleY(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(()=>ShopHelpText.SetActive(true)));
            feedback.PlayFeedbacks();
        }
    }
}
