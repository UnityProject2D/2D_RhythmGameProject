using System.Collections.Generic;
using UnityEngine;

public class EnemyPatternBuffer : MonoBehaviour
{
    public static EnemyPatternBuffer Instance { get; private set; }

    private Queue<int> patternQueue = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EnqueuePattern(int index)
    {
        patternQueue.Enqueue(index);
    }

    public bool TryDequeue(out int index)
    {
        if (patternQueue.Count > 0)
        {
            index = patternQueue.Dequeue();
            return true;
        }
        index = -1;
        return false;
    }
}
