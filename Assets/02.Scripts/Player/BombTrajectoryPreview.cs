using System.Collections.Generic;
using UnityEngine;

// 폭탄을 충전하는 동안, 날아갈 포물선 경로를 점선으로 미리 보여준다.
[RequireComponent(typeof(LineRenderer))]
public class BombTrajectoryPreview : MonoBehaviour
{
    private const int HitBufferSize = 8;

    [SerializeField] private PlayerBombFire _bombFire;

    [Header("시뮬레이션")]
    [SerializeField] private float _timeStep = 0.05f;   // 한 점당 시뮬레이션 시간 간격
    [SerializeField] private int _maxPoints = 100;      // 최대 점 개수 (= 최대 _timeStep * _maxPoints 초)
    [SerializeField] private float _bombRadius = 0.2f;  // 충돌 판정에 쓰는 폭탄 반지름

    [Header("점선 모양")]
    [SerializeField] private float _lineWidth = 0.08f;
    [SerializeField] private float _dotSpacing = 0.2f;  // 점과 점 사이 월드 거리 (점 텍스처 가로세로 비율 2.5:1에 맞춘 값)
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private Gradient _lineColor;

    private LineRenderer _line;
    private readonly List<Vector3> _points = new List<Vector3>();
    private readonly RaycastHit[] _hitBuffer = new RaycastHit[HitBufferSize];

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.useWorldSpace = true;
        _line.textureMode = LineTextureMode.Tile;
        _line.textureScale = new Vector2(1f / _dotSpacing, 1f);
        _line.widthMultiplier = _lineWidth;
        _line.numCapVertices = 0;
        _line.alignment = LineAlignment.View;
        _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _line.receiveShadows = false;
        _line.colorGradient = _lineColor;
        if (_lineMaterial != null) _line.sharedMaterial = _lineMaterial;
        _line.enabled = false;
    }

    // 발사 지점(팔 본)은 애니메이션이 반영된 뒤의 위치가 필요하므로 LateUpdate에서 그린다.
    private void LateUpdate()
    {
        if (GameManager.Instance.GameState != GameState.Playing || !_bombFire.IsAiming)
        {
            _line.enabled = false;
            return;
        }

        Simulate();
        _line.positionCount = _points.Count;
        _line.SetPositions(_points.ToArray());
        _line.enabled = true;
    }

    // 중력만 받는 포물선을 일정 시간 간격으로 시뮬레이션하고, 무언가에 부딪히면 거기서 멈춘다.
    private void Simulate()
    {
        _points.Clear();

        Vector3 position = _bombFire.FirePoint.position;
        Vector3 velocity = _bombFire.GetThrowVelocity();
        _points.Add(position);

        for (int i = 1; i < _maxPoints; i++)
        {
            Vector3 next = position + velocity * _timeStep + 0.5f * Physics.gravity * (_timeStep * _timeStep);
            velocity += Physics.gravity * _timeStep;

            Vector3 delta = next - position;
            if (TryGetHit(position, delta, out RaycastHit hit))
            {
                // 시작 지점부터 겹쳐 있던 콜라이더는 hit.point가 (0,0,0)으로 오므로 시작 위치를 쓴다.
                _points.Add(hit.distance > 0f ? hit.point : position);
                return;
            }

            _points.Add(next);
            position = next;
        }
    }

    // 플레이어 자신의 콜라이더는 무시하고, 가장 가까운 충돌 지점을 찾는다.
    private bool TryGetHit(Vector3 origin, Vector3 delta, out RaycastHit nearest)
    {
        nearest = default;
        float distance = delta.magnitude;
        if (distance <= Mathf.Epsilon) return false;

        int count = Physics.SphereCastNonAlloc(origin, _bombRadius, delta / distance, _hitBuffer, distance,
            ~0, QueryTriggerInteraction.Ignore);

        float nearestDistance = float.MaxValue;
        bool found = false;
        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = _hitBuffer[i];
            if (hit.collider.transform.IsChildOf(_bombFire.transform)) continue;

            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                nearest = hit;
                found = true;
            }
        }

        return found;
    }
}
