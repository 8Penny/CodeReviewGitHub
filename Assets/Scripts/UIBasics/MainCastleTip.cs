using System;
using UnityEngine;

namespace UIBasics
{
    public class MainCastleTip : MonoBehaviour
    {
        private float SHIFT = 100;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private ScrollWorldComponent _scroll;
        [SerializeField]
        private Transform _mainCastleTransform;
        [SerializeField]
        private RectTransform _tip;
        [SerializeField]
        private RectTransform _tipParent;

        private bool _needShow;
        private Vector2 _position;
        private Vector2 _screenSize;
        private Vector2 _tipPosition;
        
        private Vector2 _empty = new Vector2(-1000, -1000);

        private void Awake()
        {
            
            _screenSize = new Vector2(Screen.width, Screen.height);
#if UNITY_EDITOR
            _screenSize = new Vector2(1080, 1920);
#endif
        }

        private void Update()
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(_mainCastleTransform.position);
            Vector2 newTipPosition;
            if (screenPos.x > _screenSize.x + SHIFT || screenPos.x < -SHIFT || screenPos.y < -SHIFT || screenPos.y > _screenSize.y+ SHIFT)
            {
                float x = Mathf.Clamp(screenPos.x, 0, _screenSize.x) / _screenSize.x;
                float y = Mathf.Clamp(screenPos.y, 0, _screenSize.y) / _screenSize.y;
                newTipPosition = new Vector2(x * _tipParent.rect.width ,y * _tipParent.rect.height);
            }
            else
            {
                newTipPosition = _empty;
            }

            if ((_tipPosition - newTipPosition).magnitude < 0.1f)
            {
                return;
            }

            _tipPosition = newTipPosition;
            _tip.anchoredPosition = _tipPosition;
        }

        public void OnTipClicked()
        {
            _scroll.FocusOnMain();
        }
    }
}