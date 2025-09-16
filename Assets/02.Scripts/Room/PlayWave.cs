using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlayWave : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float Speed;

    [Header("Wave Pair (Left, Right)")]
    public GameObject[] Waves;

    private float _gapX;
    private float _moveValue = 0;


    private WaveFunction[] _waveSolvers;
    private Transform[] _waveTrans;
    

    private void Awake()
    {
        SettingComponent();
    }
    private void Start()
    {
        SettingValue();
    }

    private void SettingComponent()
    {
        _waveTrans = new Transform[2]
        {
             Waves[0].transform,
             Waves[1].transform
        };

        _waveSolvers = new WaveFunction[2]
        {
            Waves[0].GetComponentInChildren<WaveFunction>(),
            Waves[1].GetComponentInChildren<WaveFunction>()
        };
    }

    private void SettingValue()
    {
        _waveSolvers[0].StartSolve();
        _waveSolvers[1].StartSolve();

        _gapX = Mathf.Abs(_waveSolvers[1].transform.position.x - _waveSolvers[0].transform.position.x);
    }
    private void Update()
    {
        TileMove();
    }

    private void TileMove()
    {
        _waveTrans[0].Translate(Vector3.left * Time.deltaTime * Speed);
        _waveTrans[1].Translate(Vector3.left * Time.deltaTime * Speed);

        // 누적 이동
        _moveValue += Time.deltaTime * Speed;

        // gap만큼 이동
        if (_gapX <= _moveValue)
        {
            int idx = _waveTrans[0].position.x <= _waveTrans[1].position.x ? 0 : 1;

            // wave 뒤에꺼 변경
            if (_waveTrans[idx].position.x < _waveTrans[Mathf.Abs((idx - 1))].position.x)
            {
                _waveTrans[idx].transform.Translate(_gapX * 2, 0.0f, 0.0f);
                _waveSolvers[idx].StartSolve();
            }

            // 누적 이동 리셋(오차 방지를 위해 _gapX만큼 빼기)
            _moveValue -= _gapX;
        }
    }
    public void ChangeObject()
    {
        Vector3 waveP0 = _waveTrans[0].position;
        Vector3 waveP1 = _waveTrans[1].position;
        if (waveP0.x < waveP1.x) // 0번이 왼쪽
        {
            _waveTrans[0].position = waveP1;

            _waveSolvers[0].StartSolve();
            Debug.Log("test: 0번 Solve");
        }
        else
        {
            _waveTrans[1].position = waveP0;

            _waveSolvers[1].StartSolve();
            Debug.Log("test: 1번 Solve");
        }
    }
}
