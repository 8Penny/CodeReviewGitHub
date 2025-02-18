using System;
using System.Collections;
using DG.Tweening;
using Static;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif

namespace Services.Paths
{
    public class PathView : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _nodes;
        [SerializeField]
        private int _ceil = 45;
        [SerializeField]
        private float _floor = -1.3f;
        [SerializeField]
        private float _fallDuration = 0.1f;
        [SerializeField]
        private float _delayDuration = 0.005f;
        [SerializeField]
        private Ease _fallEase;

        [SerializeField]
        private GameObject _staticMeshesHolder;

        
        private Sequence _sequence;
        private Coroutine _distanceCoroutine;
        private float _currentLenght;
        private int _debug;

        public Vector3 FirstNode => _nodes == null? Vector3.zero : _nodes[0].position;
        
        private void Awake()
        {
            Show();
        }
        
#if UNITY_EDITOR
        [Button]
        public void UpdateNodes()
        {
            _nodes = null;
            Transform[] children = GetComponentsInChildren<Transform>();
            if (children.Length < 2)
            {
                return;
            }
            children.Sort((l, r) =>
            {
                if (l == null) { return -1; }
                if (r == null) { return 1; }

                var distance1 = Vector3.Distance(l.position, transform.position);
                var distance2 = Vector3.Distance(r.position, transform.position);
				
                if (distance1 > distance2) { return 1; }
                if (distance1 < distance2) { return -1; }
                return 0;
            });

            _nodes = new Transform[children.Length - 1];
            for (int i = 1; i < children.Length; i++)
            {
                _nodes[i-1] = children[i];
                children[i].SetSiblingIndex(i-1);
            }
        }

        #endif
        public void Hide()
        {
            ResetPathPosition(StaticValues.GroundYPos +_ceil, false);
            if (_staticMeshesHolder == null)
            {
                return;
            }
            _staticMeshesHolder.SetActive(false);
        }
        
        public void Show()
        {
            if (_distanceCoroutine != null)
            {
                return;
            }
            if (_sequence?.active ?? false)
            {
                return;
            }
            ResetPathPosition(StaticValues.GroundYPos + _floor, false);
            if (_staticMeshesHolder == null)
            {
                return;
            }
            _staticMeshesHolder.SetActive(true);
        }
        
#if UNITY_EDITOR
        [Button]
        #endif
        public void TestPlay()
        {
            Hide();
            Play(MakeMeshesStatic);
        }

        public void PlayByDistance(Transform movingObject)
        {
            if (_distanceCoroutine != null)
            {
                StopCoroutine(_distanceCoroutine);
                _distanceCoroutine = null;
            }

            _distanceCoroutine = StartCoroutine(WaitForCloserDistance(movingObject));
        }

        private IEnumerator WaitForCloserDistance(Transform movingObject)
        {
            float start = Time.time;
            while (true)
            {
                Vector3 updatedPosition = new Vector3(movingObject.position.x, FirstNode.y, movingObject.position.z);
                if ((updatedPosition - FirstNode).magnitude < 3.6f || Time.time - start > 5f)
                {
                    Play(MakeMeshesStatic);
                    yield break;
                }

                yield return null;
            }
        }
        private void Play(Action callback = null)
        {
            _sequence = DOTween.Sequence();
            for (int i = 0; i < _nodes.Length; i++)
            {
                GameObject go = _nodes[i].gameObject;
                if (i == 0)
                {   
                    _sequence.Append(_nodes[i].DOMoveY(StaticValues.GroundYPos +_floor, _fallDuration).SetEase(_fallEase));
                    UpdateVisibility(go, true);
                }
                else
                {
                    _sequence.Insert(i * _delayDuration, _nodes[i]
                        .DOMoveY(StaticValues.GroundYPos + _floor, _fallDuration)
                        .OnStart(() =>
                        {
                            UpdateVisibility(go, true);
                        })
                        .SetEase(_fallEase));
                }
            }

            if (callback != null)
            {
                _sequence.Play().OnComplete(callback.Invoke);
                return;
            }

            _sequence.Play();
        }

        private void MakeMeshesStatic()
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].gameObject.SetActive(false);
            }

            _staticMeshesHolder.SetActive(true);
        }
        
        private void ResetPathPosition(float y, bool isVisible)
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
            for (int i = 0; i < _nodes.Length; i++)
            {
                var current = _nodes[i].position;
                _nodes[i].position = new Vector3(current.x, y, current.z);
                UpdateVisibility(_nodes[i].gameObject, isVisible);
            }
        }

        private void UpdateVisibility(GameObject gameObject, bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}