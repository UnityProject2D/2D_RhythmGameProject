using MoreMountains.Feedbacks;
using UnityEngine;

public class VFXStarter : MonoBehaviour
{
    public bool[] flags;
    void Start()
    {

        VFXManager.Instance.SetOnMissFeedback(GetComponent<MMF_Player>());
    }
}
