using UnityEngine;

public class SquareOrbitEffect : MonoBehaviour
{
    public RectTransform targetIcon; // 중심 아이콘
    private RectTransform rect;
    public float tSize; // 아이콘의 외곽 경로 크기
    public float speed; // 움직이는 속도 (픽셀/초)

    private Vector3[] pathPoints;
    [SerializeField]private int currentIndex = 0;

    private void OnEnable()
    {
        Vector3 center = targetIcon.anchoredPosition;
        float size = tSize / 2;
        // 아이콘 중심 기준으로 4개 꼭지점 생성
        pathPoints = new Vector3[]
        {
            center + new Vector3(-size, size),  // Top Left
            center + new Vector3(size, size),   // Top Right
            center + new Vector3(size, -size),  // Bottom Right
            center + new Vector3(-size, -size)  // Bottom Left
        };

        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = pathPoints[currentIndex];
    }

    private void Update()
    {
        if(!gameObject.activeSelf) return;
        if (pathPoints == null || pathPoints.Length == 0) return;

        Vector3 target = pathPoints[currentIndex];
        rect.anchoredPosition = Vector3.MoveTowards(rect.anchoredPosition, target, speed * Time.deltaTime);

        if (Vector3.Distance(rect.anchoredPosition, target) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % pathPoints.Length;
        }
    }
}
