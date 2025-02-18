using System;
using DG.Tweening;
using Static;
using UnityEngine;
using UnityEngine.EventSystems;


    public class ScrollWorldComponent : MonoBehaviour
    {
        private float SIZE_MIN;
        private float SIZE_MAX;
        
        [SerializeField]
        private TouchComponent _touch;
        [SerializeField]
        private float _maxX;
        [SerializeField]
        private float _minX;
        [SerializeField]
        private float _maxZ;
        [SerializeField]
        private float _minZ;
        [SerializeField]
        private float _k3 = 0.2f;
        [SerializeField]
        private float _speed = 10.2f;

        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private Camera _camera;

        private bool _isDrag;
        private bool _isZoom;
        private bool _isLocked;

        private float _startDistance;
        
        private Ray _ray;
        private RaycastHit _hit;
        private Vector3 _position;
        private Vector3 _startPosition;
        private Vector2 _tapPosition;
        private Transform _target;
        private Transform _targetPoint;
        private Transform _cameraTransform;
        
        private Sequence _focusSequence;
        private Sequence _focusCastleSequence;

        public void Awake()
        {
            _cameraTransform = _camera.transform;
            _target = transform;
            _startPosition = _target.position;
            SIZE_MIN = StaticValues.GroundYPos - 35;
            SIZE_MAX = StaticValues.GroundYPos + 30;

            GameObject empty = new GameObject();
            empty.transform.SetParent(_target.parent);
            _targetPoint = empty.transform;
            _targetPoint.position = _startPosition;
        }

        private void Update()
        {
            ProcessZoom();
            ProcessDrag();
            ProcessFly();
        }

        private void ProcessZoom()
        {
            if (_isLocked)
            {
                return;
            }
#if UNITY_EDITOR

            var t = Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(t) < 0.001f)
            {
                return;
            }
            
            var newEditorPos = _targetPoint.position - 9f * _cameraTransform.forward * t;
            if (newEditorPos.y > SIZE_MAX || newEditorPos.y < SIZE_MIN)
            {
                return;
            }

            StopFocusSequence();
            _targetPoint.position = Clamp(newEditorPos);
            return;
#endif
            if (!_isZoom && _touch.IsZoomStarted())
            {
                _startDistance = Vector3.Distance(_touch.GetTapPosition(0), _touch.GetTapPosition(1));
                _isZoom = true;
                _isDrag = false;
                StopFocusSequence();
                return;
            }

            if (!_isZoom)
            {
                return;
            }

            if (_touch.IsMouseUp(0) || _touch.IsMouseUp(1))
            {
                _isZoom = false;
                return;
            }
            
            float currentDistance = Vector3.Distance(_touch.GetTapPosition(0), _touch.GetTapPosition(1));
            var newPos = _targetPoint.position - _cameraTransform.forward * (_startDistance - currentDistance) / 6f;
            
            _startDistance = currentDistance;
            if (newPos.y > SIZE_MAX || newPos.y < SIZE_MIN)
            {
                return;
            }

            
            _targetPoint.position = Clamp(newPos);

        }

        private void ProcessDrag()
        {
            if (_isLocked)
            {
                return;
            }
            if (_isZoom)
            {
                return;
            }
            
            Vector2 tapPosition = _touch.GetTapPosition();

            if (EventSystem.current != null && _touch.IsMouseDown())
            {
                _ray = _camera.ScreenPointToRay(tapPosition);
                if (_collider.Raycast(_ray, out _hit, 200))
                {
                    _isDrag = true;
                    _tapPosition = tapPosition;

                    StopFocusSequence();
                }
            }
            else if (_touch.IsMouseUp())
            {
                _isDrag = false;
            }
            else if (_isDrag && _touch.IsMouseHeld())
            {
                if ((tapPosition - _tapPosition).magnitude < 0.05f)
                {
                    return;
                }
                
                _ray = _camera.ScreenPointToRay(tapPosition);
                if (_collider.Raycast(_ray, out _hit, 200))
                {
                    var delta =  _tapPosition - tapPosition;
                    float distanceMultiplier = (_targetPoint.position.y - SIZE_MIN) / (SIZE_MAX - SIZE_MIN);
                    float divider = Mathf.Lerp(30f, 12f, distanceMultiplier);
                    _position = _targetPoint.position + new Vector3(delta.x, 0, delta.y) / divider;
                    _tapPosition = tapPosition;
                    _targetPoint.position = Clamp(_position);
                }
            }
        }
        private float EaseOutExpo(float x){
            return x >= 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
        }

        private void ProcessFly()
        {
            if (_isLocked)
            {
                return;
            }
            if (Vector3.Distance(_targetPoint.position,  _target.position) < 0.1f)
            {
                return;
            }

            Vector3 delta = _targetPoint.position - _target.position;
            float speed = Mathf.Lerp(_speed/2f, _speed*3, EaseOutExpo(Mathf.Min(1f, delta.magnitude / 50f)));
            _target.position += Mathf.Min(Time.deltaTime * speed, delta.magnitude) * delta.normalized;
        }

        public void FocusOnMain()
        {
            if (_focusSequence?.active ?? false)
            {
                return;
            }
            _focusSequence = DOTween.Sequence();
            _focusSequence.Append(_targetPoint.DOMove(_startPosition, 0.5f));
            _focusSequence.Play();
        }

        private void StopFocusSequence()
        {
            _focusSequence?.Kill();
        }

        private Vector3 Clamp(Vector3 raw) 
        {
            float k1 = 1 + (SIZE_MAX - _targetPoint.position.y) / (SIZE_MAX - SIZE_MIN);
            float k2 = k1 - _k3;
            k2 = Mathf.Max(k2, 1, k2);
            return new Vector3(Mathf.Clamp(raw.x, k2 * _minX, k2 * _maxX), raw.y, Mathf.Clamp(raw.z, k1 * _minZ, k1 * _maxZ));
        }

        public void FocusFirstCastle(float interval, Action completeAction = null)
        {
            _focusCastleSequence = DOTween.Sequence();
            _focusCastleSequence.AppendInterval(interval).OnComplete(() =>
            {
                _targetPoint.transform.DOMove(new Vector3(-26.33f, 3230, -48.08f), 1f);
                completeAction?.Invoke();
            });
            _focusCastleSequence.Play();
        }

        public void Lock()
        {
            _isLocked = true;
        }
        public void Unlock()
        {
            _isLocked = false;
        }
    }
