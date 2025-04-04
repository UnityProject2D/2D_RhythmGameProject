using UnityEngine;
using UnityEngine.UI;

public class UI_HPPanel : MonoBehaviour
{
    [SerializeField] private GameObject heartFilledPrefab; // 찬 하트 프리팹
    [SerializeField] private GameObject heartSolidPrefab; // 빈 하트 프리팹
    [SerializeField] private int maxHearts = 3; // 최대 하트 개수

    private LayoutGroup layoutGroup; // Horizontal Layout Group 캐싱

    private int currentHP; // 현재 하트 개수
    public int CurrentHP
    {
        get { return currentHP; }
        set
        {
            currentHP = Mathf.Clamp(value, 0, maxHearts); // 하트 개수 제한
            UpdateHearts(currentHP); // 하트 업데이트
        }
    }

    private void Awake()
    {
        // Horizontal Layout Group 컴포넌트 캐싱
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            Debug.LogError("HorizontalLayoutGroup 컴포넌트가 필요합니다.");
        }

        // 초기 하트 개수 설정
        currentHP = maxHearts;
    }

    private void Start()
    {
        // 게임 시작 시 하트 초기화
        InitializeHearts();
    }

    // 하트 초기화 메서드
    private void InitializeHearts()
    {
        // 최대 하트 개수만큼 하트 생성
        UpdateHearts(maxHearts);
    }

    public void UpdateHearts(int currentHP)
    {
        // 기존 하트 삭제
        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // 새로운 하트 생성
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartPrefab = (i < currentHP) ? heartFilledPrefab : heartSolidPrefab;
            GameObject heart = Instantiate(heartPrefab, layoutGroup.transform);
            heart.transform.SetParent(layoutGroup.transform, false);
        }
    }

    // 테스트용: 키 입력으로 하트 개수 변경
    private void TestHeartControls()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CurrentHP++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CurrentHP--;
        }
    }
}
