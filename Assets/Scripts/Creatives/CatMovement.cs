
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _cat;
    [SerializeField]
    private float _speed = 1f;
    
    private BezierSpline _curve;
    private float _progress;
    private void Awake()
    {
        _curve = GetComponent<BezierSpline>();
    }

    private void Update()
    {
        _progress += Time.deltaTime * _speed;
        if (_progress > 1f)
        {
            _progress = 0f;
        }
        Vector3 next = _curve.GetPoint(_progress + Time.deltaTime* _speed);
        _cat.LookAt(next);
        Vector3 finalPos = _curve.GetPoint(_progress);
        
        _cat.position = finalPos;
        
        //
        // _animator.SetFloat(SPEED, 
        //     CastleUtils.GetSpeed(_castle.ShipSpeedLevel,
        //         _talentsService.CatSpeedMultiplier * _boostService.GetPlanetBoost(_castle.Id)) *Mathf.Pow(_debugParameter, 1f/_debugPow));


    }
}
