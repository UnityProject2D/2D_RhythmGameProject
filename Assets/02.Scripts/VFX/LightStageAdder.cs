using MoreMountains.Feedbacks;
using UnityEngine;
using System.Collections.Generic;

public class LightStageAdder : MonoBehaviour
{
    private void Start()
    {
        var mmfPlayers = GetComponentsInChildren<MMF_Player>();

        VFXManager.Instance.LightMMFPlayers = new List<MMF_Player>(mmfPlayers);
    }
}
