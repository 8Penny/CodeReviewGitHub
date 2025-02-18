using System.Collections;
using Services;
using Services.Paths;
using Services.Sounds;
using Services.Tutorial;
using Services.View;
using Sirenix.OdinInspector;
using Static;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CastleView : MonoBehaviour
    {
        private float SPEED = 4f;
        
        [SerializeField]
        private int _id;

        [SerializeField]
        private Transform _planetModel;
        [SerializeField]
        private TextMeshPro _textMesh;
        [SerializeField]
        private Material _unavailableMaterial;
        [SerializeField]
        private GameObject _castleMainGO;
        [SerializeField]
        private ColliderButton _colliderButton;
        [SerializeField]
        private CatView _cat;
        [SerializeField]
        private PathView _pathView;
        [SerializeField]
        private BezierSpline _curve;
        [SerializeField]
        private ResourceHolder _resourceHolder;
        [SerializeField]
        private GameObject _dogs;
        [SerializeField]
        private GameObject _chooseCastleEffect;

        private ViewCastlesService _viewCastlesService;
        private TutorialService _tutorialService;
        private PlayerResourcesService _playerResourcesService;
        private UIService _uiService;

        private MeshRenderer _meshRenderer;
        private Castle _presenter;

        private Material[] _availableMaterials;
        private Material[] _unavailableMaterials;

        private bool _isOpen;
        public int Id => _id;
        public bool HasPresenter => _presenter != null;
        public Castle Presenter => _presenter;
        public bool CanOpen { get; set; }

        [Inject]
        public void Init(ViewCastlesService viewCastlesService, TutorialService tutorialService,
            PlayerResourcesService playerResourcesService, UIService uiService)
        {
            _uiService = uiService;
            _tutorialService = tutorialService;
            _viewCastlesService = viewCastlesService;
            _playerResourcesService = playerResourcesService;

            _viewCastlesService.Register(this);
        }
        
        public void Awake()
        {
            _chooseCastleEffect.gameObject.SetActive(false);
            _meshRenderer = _planetModel.GetComponentInChildren<MeshRenderer>();
            _availableMaterials = _meshRenderer.materials;
            _unavailableMaterials = new Material[_availableMaterials.Length];

            for (int i = 0; i < _availableMaterials.Length; i++)
            {
                _unavailableMaterials[i] = _unavailableMaterial;
            }
        }

        private void UpdateChooseEffect(bool isVisible)
        {
            _chooseCastleEffect.gameObject.SetActive(isVisible && (_presenter?.IsOpen ?? false));
        }
        public void SetUnlockPrice(int price)
        {
            _textMesh.text = UiUtils.GetCountableValue(price, 0);
        }

        public void UpdateState(bool isHidden, bool isOpen)
        {
            _castleMainGO.SetActive(!isHidden);
            bool isResourcesOpen = !isHidden && isOpen;
            _isOpen = isOpen;
            _cat.gameObject.SetActive(isResourcesOpen);
            _resourceHolder.gameObject.SetActive(isResourcesOpen);
            _dogs.gameObject.SetActive(isResourcesOpen);
            
            if (isOpen)
            {
                _pathView.Show();
            } else
            {
                _pathView.Hide();
            }
            
            if (isHidden)
            {
                _textMesh.transform.parent.gameObject.SetActive(false);
                return;
            }

            _resourceHolder.UpdateVisibility(isOpen);
            var materials = isOpen ? _availableMaterials : _unavailableMaterials;
            _meshRenderer.materials = materials;
            UpdateAvailableEffect(ResourceType.None);
            
            _textMesh.transform.parent.gameObject.SetActive(!isOpen && (_id == 0 || _tutorialService.IsComplete));
        }

        public void SetPresenter(Castle castle)
        {
            _presenter = castle;
            _cat.SetParameters(_curve, _presenter);
            _resourceHolder.SetResources(castle.Settings.Resources);
        }

        public void OnEnable()
        {
            _colliderButton.OnClick += Interact;
            _tutorialService.OnNextTutorialStep += TutorialUpdatedHandler;
            _playerResourcesService.OnResourcesUpdated += UpdateAvailableEffect;
            _uiService.OnChooseCastleStateUpdated += UpdateChooseEffect;
            TutorialUpdatedHandler(0);
            UpdateChooseEffect(_uiService.ChoosingCastle);
        }
        
        public void OnDisable()
        {
            _colliderButton.OnClick -= Interact;
            _tutorialService.OnNextTutorialStep -= TutorialUpdatedHandler;
            _playerResourcesService.OnResourcesUpdated -= UpdateAvailableEffect;
            _uiService.OnChooseCastleStateUpdated -= UpdateChooseEffect;
        }

        private void TutorialUpdatedHandler(int _)
        {
            if (_id != 0)
            {
                _textMesh.transform.parent.gameObject.SetActive(!_isOpen && _tutorialService.IsComplete);
            }
            _textMesh.color = Id != 0 && !_tutorialService.IsComplete ? new Color(1,1,1,0.5f): Color.white;
            UpdateAvailableEffect(ResourceType.None);
        }

        private void Interact()
        {
            if (_id != 0 && !_isOpen && !_tutorialService.IsComplete)
            {
                return;
            }
            _viewCastlesService.InteractCastle(_id);
        }

        public void PlayOpen()
        {
            StartEffect();
        }

        private void StartEffect()
        {
            StartCoroutine(PlayVfx());
        }

        private IEnumerator PlayVfx()
        {
            _pathView.PlayByDistance(_cat.transform);
            CanOpen = true;
            // _shaderObject.CollectMaterials();
            // _shaderObject.StartSonarRing(_planetModel.position + Vector3.up*3.8f, 45f );//_particle.Play();

            //_particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            yield return null;
        }
        

        private void UpdateAvailableEffect(ResourceType resourceType)
        {
            if (resourceType != ResourceType.None || _presenter == null )
            {
                return;
            }

            bool canBeShownIfTutorial = _tutorialService.IsComplete || _presenter.Id == 0;
            bool hasEffect = canBeShownIfTutorial && !_presenter.IsOpen && _playerResourcesService.CanBuy(new ResourceDemand(ResourceNames.Soft, _presenter.Settings.UnlockPrice));

        }

        [Button]
        public void CalculateDistance()
        {
            float result = 0;
            Vector2 previous = GetV(_curve.points[0]);
            for (int i = 3; i+3 < _curve.points.Length; i+=3)
            {
                Vector2 next = GetV(_curve.points[i]);
                result += Vector2.Distance(previous, next);
                previous = next;
            }
            Debug.Log(result);
        }

        private Vector2 GetV(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
    }
}