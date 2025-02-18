using UnityEngine;
using UnityEngine.EventSystems;


    public class TouchComponent : MonoBehaviour
    {
#if UNITY_EDITOR

        public Vector2 GetTapPosition(int index = 0)
        {
            return Input.mousePosition;
        }

        public bool IsMouseDown(int index = 0)
        {
            return Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
        }
        public bool IsUIMouseDown(int index = 0)
        {
            return Input.GetMouseButtonDown(0);
        }

        public bool IsMouseUp(int index = 0)
        {
            return Input.GetMouseButtonUp(0);
        }

        public bool IsMouseHeld(int index = 0)
        {
            return Input.GetMouseButton(0);
        }
        
        public bool IsZoomStarted(){
            return false;
        }

#else
        public Vector2 GetTapPosition(int index = 0) {
            return Input.touchCount > index ? Input.touches[index].position : Vector2.zero;
        }

        public bool CheckTouchPhase(TouchPhase phase, int index = 0) {
            return Input.touchCount > index && Input.touches[index].phase == phase;
        }

        public bool IsZoomStarted()
        {
            bool isFirstActive = !CheckTouchPhase(TouchPhase.Ended) && !CheckTouchPhase(TouchPhase.Canceled);
            return isFirstActive && CheckTouchPhase(TouchPhase.Began, 1);
        }

        public bool IsMouseDown(int index = 0) {
            return CheckTouchPhase(TouchPhase.Began, index) && !EventSystem.current.IsPointerOverGameObject(Input.touches[index].fingerId);
        }

        public bool IsUIMouseDown(int index = 0) {
            return CheckTouchPhase(TouchPhase.Began, index);
        }

        public bool IsMouseUp(int index = 0) {
            return CheckTouchPhase(TouchPhase.Ended, index);
        }

        public bool IsMouseHeld(int index = 0) {
            return CheckTouchPhase(TouchPhase.Moved, index);
        }
#endif
    }
