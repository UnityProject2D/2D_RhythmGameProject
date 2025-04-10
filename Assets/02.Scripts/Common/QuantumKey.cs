using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class QuantumKey : MonoBehaviour
{
    public MMF_Player DropFeedback;
    public MMF_Player PickUpFeedback;
    void Start()
    {
        MoveTowardPlayer().Forget();

        async UniTaskVoid MoveTowardPlayer()
        {
            DropFeedback?.PlayFeedbacks();
            await UniTask.Delay(1000);
            PickUpFeedback?.PlayFeedbacks();

            await UniTask.Delay(500);
            Move().Forget();
        }
    }

    private async UniTaskVoid Move()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * 0.6f, t / 4);
            await UniTask.Yield();
        }

        CurrencyManager.Instance.Add(CurrencyType.QuantumKey, 1);

        //임시
        CurrencyManager.Instance.Add(CurrencyType.Credit, Random.Range(2,5));
        //

        Destroy(gameObject);
    }
}
