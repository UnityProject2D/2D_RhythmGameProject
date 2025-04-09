using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    [Header("아이템 관련")]
    [Space(10)]
    [Header("영구 아이템")]
    [Space(5)]
    [Header("리커버리 코어: Perfect시 일정 확률로 체력 n 회복")]
    public bool RecoveryCoreEnabled;
    public float RecoveryCoreAmount;
    [Header("위기 대응 코어: HP p% 미만일 때 데미지 감소 q%")]
    public bool EmergencyResponseCoreEnabled;
    [Tooltip("위기 대응 코어 체력 비율")]
    public float EmergencyResponseCoreThreshold;
    [Tooltip("위기 대응 코어 데미지 감소 비율")]
    public float EmergencyResponseCoreReduce;
    [Header("접속권한 레벨 코어: 상점 아이템 20% 할인")]
    public bool AccessLevelCoreEnabled;
    [Header("회복 알고리즘: n스테이지마다 체력 회복")]
    public bool RecoveryAlgorithmCoreEnabled;

    [Space(10)]
    [Header("장비 아이템")]
    [Space(5)]
    [Header("자동 교정 유닛: 체력 깎이는거 한번 방어")]
    public bool CalibrationChipsetEnabled;
    [Header("강제 회피 프로토콜: 미스해도 일정확률로 Good 판정")]
    public bool ForcedEvasionEnabled;
    [Header("자동 콤보 시스템: Perfect 3회마다 추가 콤보 누적")]
    public bool AutoComboSystemEnabled;
    [Header("정밀 보정 칩셋: Bad도 콤보 유지, 기본 점수 획득량 감소")]
    public bool PreciseCalibrationUnitEnabled;
    [Header("보너스 칩: Perfect시 20% 증가된 점수 획득")]
    public bool BonusChipEnabled;
    [Header("골드 획득기?: 스테이지 클리어시 돈 추가 획득")]

    [Space(10)]
    [Header("소비 아이템")]
    [Space(5)]
    //패턴 체계화: 적 공격 패턴이 조금 더 일관된 템포로 단순화
    [Header("긴급 회피 프로토콜: HP 0 될 경우 1회 생존 (자동 사용)")]
    public bool EmergencyEvasionEnabled;
    [Tooltip("긴급 회피 프로토콜 체력 회복량")]
    public float EmergencyEvasionRecoveryAmount;
    [Header("오버 드라이브: 퍼펙트시 2배 점수, Bad일때 체력 소모")]
    public bool OverDriveUsed;
    [Header("확률 증폭제: 다음 보상 확률 2배")]
    public bool ProbabilityAmplifierUsed;
    [Header("전투 리듬 캐쳐: 미스 판정 시에도 콤보 유지 (일회성)")]
    public bool ComboProtectorUsed;
    [Header("해킹 툴: 상점 아이템 랜덤 1개 무료")]
    public bool HackingToolUsed;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
