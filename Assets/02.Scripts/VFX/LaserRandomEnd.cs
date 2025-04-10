using UnityEngine;

public class LaserRandomEnd : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject LaserEnd;
    public GameObject []LaserExplosion;
    public float range = 1f; // 랜덤 이동 범위
    public Vector3 targetPosition; // 목표 위치

    void OnEnable()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        RandomizeLaserEnd(targetPosition);
    }

    void RandomizeLaserEnd(Vector3 targetPosition)
    {
        if (lineRenderer.positionCount < 2) return;

        // 시작 위치 (Position[0])
        Vector3 start = lineRenderer.GetPosition(0);

        // 끝 위치 기준으로 랜덤 오프셋 적용
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range, range)
        );
        LaserEnd.transform.position = targetPosition + randomOffset; 
        foreach(var laserExplosion in LaserExplosion)
        {
            laserExplosion.transform.position = targetPosition + randomOffset;
        }
        Vector3 newEnd = start + targetPosition + randomOffset;
        lineRenderer.SetPosition(1, newEnd);
    }
}