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
    public bool IsShop = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            image.GetComponent<RectTransform>().DOScaleX(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(()=>image.GetComponent<RectTransform>().DOScaleY(1, 0.2f).SetEase(Ease.InOutExpo).OnComplete(()=>ShopHelpText.SetActive(true)));
            feedback.PlayFeedbacks();
            IsShop = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShopHelpText.SetActive(false);
            image.GetComponent<RectTransform>().DOScaleY(0.01f, 0.05f).SetEase(Ease.InOutExpo).OnComplete(() => image.GetComponent<RectTransform>().DOScaleX(0, 0.05f).SetEase(Ease.InOutExpo));
            feedback.PlayFeedbacks();
            IsShop = false;
        }
    }
}
