using System;
using Services;
using Services.Updater;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CastlePriceComponent : MonoBehaviour, IUpdatable
    {
        private Transform _mainCamera;
        private UpdateService _updateService;
        private UIService _uiService;
        private Transform _transform;
        private Vector3 _current;

        [Inject]
        public void Init(UpdateService updateService, UIService uiService)
        {
            _updateService = updateService;
            _transform = transform;
            _current = _transform.position;
            _uiService = uiService;
        }

        private void OnEnable()
        {
            _updateService.Register(this);
        }
        private void OnDisable()
        {
            _updateService.Unregister(this);
        }
        void IUpdatable.Update()
        {
            if (_mainCamera == null)
            {
                if (_uiService.Views == null)
                {
                    return;
                }
                _mainCamera = _uiService.Views.Camera.transform;
            }
            Vector3 cameraPosition = _mainCamera.position;
            Vector3 specialPosition = new Vector3(_current.x, cameraPosition.y, cameraPosition.z);
            transform.rotation = Quaternion.LookRotation( _current - specialPosition );
        }
    }
}