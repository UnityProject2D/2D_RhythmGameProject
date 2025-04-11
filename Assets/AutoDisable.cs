using System.Collections;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float time;
    void OnEnable()
    {
        StartCoroutine(Disable(time));
    }

    IEnumerator Disable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
