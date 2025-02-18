using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Views
{
    [RequireComponent(typeof(Collider))]
    public class ColliderButton : MonoBehaviour
    {
        public Action OnClick;

        private bool _isPress;
        private float _pressTime;

        private Vector2 _positionPress;

        public void OnMouseDown()
        {
            _isPress = true;
            _pressTime = 0;
            _positionPress = Input.mousePosition;
        }
        
        public void OnMouseUp()
        {
            Vector2 position = Input.mousePosition;
            var delta = Time.deltaTime;
            if (delta < 0.2f)
            {
                delta = 0.2f;
            }

            if (_pressTime < delta && (position - _positionPress).sqrMagnitude < 900 && OnClick != null)
            {
#if UNITY_EDITOR
                var isUiOverride = EventSystem.current.IsPointerOverGameObject();
#else
                
                if (Input.touchCount == 0)
                {
                    return;
                }

                var touch = Input.touches[0];
                var isUiOverride = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
#endif
                if (!isUiOverride)
                {
                    OnClick.Invoke();
                }
            }

            _isPress = false;
        }

        private void Update()
        {
            if (_isPress)
            {
                _pressTime += Time.deltaTime;
            }
        }
    }
}