using System.Collections;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    private Transform EnemyTransform;
    void Start()
    {
        if (GameManager.Instance.Target != null)
        {
            EnemyTransform = GameManager.Instance.Target.transform;
            StartCoroutine(FollowEnemy());
        }
    }

    IEnumerator FollowEnemy()
    {
        while (EnemyTransform != null)
        {
            transform.position = new Vector3(EnemyTransform.position.x, EnemyTransform.position.y);
            yield return new WaitForSeconds(0.4f);
        }
    }
}
