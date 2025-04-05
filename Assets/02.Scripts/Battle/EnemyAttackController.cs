using System.Collections;
using UnityEngine;
using static RhythmEvents;

public enum EnemyAttackState
{
    Idle,
    Shoot1,
    Shoot2,
    Shoot3,
    Shoot4
}

public class EnemyAttackController : MonoBehaviour
{
    private Animator _animator;

    public GameObject[] EnemyAttackPrefabs;
    public Transform[] EnemyAttackSpawnPoints;
    private GameObject[] attackPool = new GameObject[4];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            attackPool[i] = Instantiate(EnemyAttackPrefabs[i]);
            attackPool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        OnBeat += OnBeatReceived;
    }

    private void OnDisable()
    {
        OnBeat -= OnBeatReceived;
    }

    private void OnBeatReceived(float beatTime)
    {
        if (!EnemyPatternBuffer.Instance.TryDequeue(out int index)) return;

        _animator.SetInteger("Direction", index + 1);
        _animator.SetTrigger("Attack");

        GameObject attack = attackPool[index];
        attack.transform.position = EnemyAttackSpawnPoints[index].position;
        attack.SetActive(true);
        StartCoroutine(HidePatternAfterDelay(attack, 1f));

        StartCoroutine(ResetAnimation());
    }

    private IEnumerator HidePatternAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetInteger("Direction", 0);
    }



}
