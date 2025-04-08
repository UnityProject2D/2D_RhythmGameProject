using System.Collections;
using TMPro;
using UnityEngine;
using static RhythmInputHandler; // 플레이어 입력 처리

public enum RhythmAction
{
    None = 0, // 기본값
    Jump,
    Slide,
    Roll,
    BackFlip,
    Hit,
    Die
}

public class PlayerController : MonoBehaviour
{
    // 목표: 플레이어가 리듬 게임에서 입력을 받아서 적절한 행동을 수행하도록 하는 것
    // 플레이어는 기본적으로 애니메이션하면서 총을 쏜다.
    private Animator _animator;
    //public event Action<string> OnInputPerformed;
    public GameObject[] vfxPrefabs; // 0: Jump, 1: Down, 2: Roll, 3: BackFlip
    public Transform vfxSpawnPoint;

    public bool IsDead;
    public bool IsAlive = true;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        RhythmEvents.OnInputJudged += OnInputJudg; // 리듬 입력 판정 이벤트 구독
    }

    private void Start()
    {
        Instance.OnInputPerformed += OnInputPerf;
    }

    private void OnDisable()
    {
        Instance.OnInputPerformed -= OnInputPerf;
        RhythmEvents.OnInputJudged -= OnInputJudg; // 리듬 입력 판정 이벤트 구독 해제
    }

    private void OnInputPerf(string key)
    {
        if (IsDead) return;
        RhythmAction direction = RhythmAction.None;
        //Debug.Log($"인풋 들어감");
        switch (key)
        {
            case "W": direction = RhythmAction.Jump; break;
            case "S": direction = RhythmAction.Slide; break;
            case "A": direction = RhythmAction.Roll; break;
            case "D": direction = RhythmAction.BackFlip; break;
        }

        _animator.SetInteger("Direction", (int)direction);
        _animator.SetTrigger("Actioned");
        //StartCoroutine(ResetAnimation()); // 애니메이션 재생

        // VFX도 재생
        PlayVFX(direction);
    }

    private void PlayVFX(RhythmAction dir)
    {
        int index = (int)dir - 1;
        if (index < 0 || index >= vfxPrefabs.Length) return;

        GameObject vfx = Instantiate(vfxPrefabs[index], vfxSpawnPoint.position, Quaternion.identity);
        Destroy(vfx, 1f); // 1초 후 자동 삭제
    }

    //private IEnumerator ResetAnimation()
    //{
    //    yield return new WaitForSeconds(0.5f); // 애니메이션 재생 시간
    //    _animator.SetInteger("Direction", 0); //(int)RhythmAction.None
    //}

    private void OnInputJudg(JudgedContext result)
    {
        Debug.Log($"판정 결과: {result.Result}");
        switch (result.Result)
        {
            case JudgementResult.Perfect:
            case JudgementResult.Good:
                ShowComboEffect(result.Result); // 효과 출력
                break;
            case JudgementResult.Bad:
            case JudgementResult.Miss:
                ShowMissEffect(); //대미지 애니메이션 출력
                break;
        }
    }

    private void ShowComboEffect(JudgementResult result)
    {
        // 판정 결과에 따라 색/이펙트 다르게 출력 가능
        switch (result)
        {
            case JudgementResult.Perfect:
                Debug.Log("Perfect!!");
                break;
            case JudgementResult.Good:
                Debug.Log("Good!!");
                break;
        }
    }

    private void ShowMissEffect()
    {
        //Debug.Log("Miss!!");
        //RhythmAction direction = RhythmAction.None;
        //direction = RhythmAction.Hit;

        if(!IsDead)
        {
            _animator.SetTrigger("Hit");
        }
        //StartCoroutine(ResetAnimation());
    }

}