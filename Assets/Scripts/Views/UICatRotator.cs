using Services;
using UnityEngine;
using Zenject;

namespace Views
{
    public class UICatRotator : MonoBehaviour
    {
        [SerializeField]
        private Material[] _materials;
        [SerializeField]
        private SkinnedMeshRenderer _catMeshRenderer;
        [SerializeField]
        private TouchComponent _touch;
        [SerializeField]
        private Transform _cat;
        [SerializeField]
        private float _speed = 5f;

        private UIService _uiService;
        private RectTransform _rect;

        private bool _isActive;
        private Vector2 _previousPosition;
        private Quaternion _startRotation;

        [Inject]
        public void Init(UIService uiService)
        {
            _uiService = uiService;

            _uiService.OnViewsRegistered += Register;
        }

        private void Register()
        {
            _uiService.Views.CastlePanelView.OnPanelUpdated += UpdateCatMaterial;
        }

        private void OnDestroy()
        {
            _uiService.OnViewsRegistered -= Register;
            _uiService.Views.CastlePanelView.OnPanelUpdated -= UpdateCatMaterial;
        }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _startRotation = _cat.rotation;
        }

        private void OnEnable()
        {
            _cat.rotation = _startRotation;
        }

        private void UpdateCatMaterial(int id)
        {
            _catMeshRenderer.material = _materials[id];
        }

        private void Update()
        {
            if (_touch.IsMouseUp())
            {
                _isActive = false;
                return;
            }
            if (_touch.IsUIMouseDown())
            {
                _previousPosition = _touch.GetTapPosition();
                Vector2 localMousePosition = _rect.InverseTransformPoint(Input.mousePosition);
                _isActive = _rect.rect.Contains(localMousePosition);
                return;
            }

            if (_isActive)
            {
                Vector2 current = _touch.GetTapPosition();
                _cat.Rotate(Vector2.up, (_previousPosition.x - current.x) * _speed);
                _previousPosition = current;
            }
        }
    }
}