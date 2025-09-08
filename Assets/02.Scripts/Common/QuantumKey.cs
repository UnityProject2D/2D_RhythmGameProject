using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class QuantumKey : MonoBehaviour
{
    public MMF_Player DropFeedback;
    public MMF_Player PickUpFeedback;

    private PlayerContext _player;
    void Start()
    {
        MoveTowardPlayer().Forget();
        _player = GameManager.Instance.Player;
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
            transform.position = Vector3.Lerp(transform.position, new Vector3(-6.06f,-1.9f), t / 4); //_player.Transform.position, t / 4);
            await UniTask.Yield();
        }

        
        CurrencyManager.Instance.Add(CurrencyType.QuantumKey, 1);

        //임시

        int maxCredit = 3;
        int minCredit = 1;
        if (PlayerState.Instance.DataCacheModuleEnabled)
        {
            maxCredit += 1;
            minCredit += 1;
        }

        if (PlayerState.Instance.ProbabilityAmplifierUsed)
        {

            if (Random.value > 0.3f)
            {
                CurrencyManager.Instance.Add(CurrencyType.Credit, Random.Range(minCredit+2, maxCredit+2));
            }
            else
            {
                CurrencyManager.Instance.Add(CurrencyType.Credit, Random.Range(minCredit, maxCredit));
            }
            PlayerState.Instance.ProbabilityAmplifierUsed = false;

        }
        else
        {
            if (Random.value > 0.6f)
            {
                CurrencyManager.Instance.Add(CurrencyType.Credit, Random.Range(minCredit + 2, maxCredit + 2));
            }
            else
            {
                CurrencyManager.Instance.Add(CurrencyType.Credit, Random.Range(minCredit, maxCredit));
            }
        }
            
        //

        Destroy(gameObject);
    }
}
