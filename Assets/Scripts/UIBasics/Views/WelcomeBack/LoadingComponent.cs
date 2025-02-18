using System;
using UnityEngine;

namespace UIBasics.Views.WelcomeBack
{
    public class LoadingComponent : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private float _speed;

        private void Update()
        {
            _rectTransform.rotation = Quaternion.Euler(0,0, _rectTransform.rotation.eulerAngles.z + _speed);
        }
    }
}