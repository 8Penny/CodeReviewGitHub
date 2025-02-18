using Services;
using Services.Tutorial;
using UI;
using UIBasics.Views.AbilityPanel;
using UIBasics.Views.Recipes;
using UIBasics.Views.ResourcesPanel;
using UIBasics.Views.ShopViews;
using UIBasics.Views.ShopWindow;
using UIBasics.Views.Sounds;
using UIBasics.Views.Tasks;
using UIBasics.Views.Tutorial;
using UIBasics.Views.WelcomeBack;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UIBasics.Views
{
    public class MainViewsContainer : MonoBehaviour
    {
        [SerializeField]
        private DataPanelView _dataPanelView;
        [SerializeField]
        private CastlePanelView castlePanelView;
        [SerializeField]
        private AbilityWindowView _abilityWindowView;
        [SerializeField]
        private RecipesWindowView _recipesWindowView;
        [SerializeField]
        private ChooseCastleBoost chooseCastleBoost;
        [SerializeField]
        private SoundsWindowView _soundsWindowView;
        [SerializeField]
        private WelcomeBackWindowView _welcomeBackWindowView;
        [SerializeField]
        private ShopWindowView _shopWindowView;
        [SerializeField]
        private ResourcePanelView _resourcePanelView;
        [SerializeField]
        private TutorialTasksWindow _tutorialTasksWindow;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Transform _canvas;
        [SerializeField]
        private Image _tutorialHole;
        [SerializeField]
        private GiftChestWindow _giftChestWindow;
        [SerializeField]
        private InfoView _infoView;
        [SerializeField]
        private InfoRowsWindow _infoRowsView;
        [SerializeField]
        private GoToShopPopup _goToShopPopup;
        [SerializeField]
        private ScrollWorldComponent _scrollWorldComponent;

        private UIService _uiService;

        public DataPanelView DataPanelView => _dataPanelView;
        public CastlePanelView CastlePanelView => castlePanelView;
        public AbilityWindowView AbilityWindowView => _abilityWindowView;
        public RecipesWindowView RecipesWindowView => _recipesWindowView;
        public ChooseCastleBoost ChooseCastleBoost => chooseCastleBoost;
        public SoundsWindowView SoundsWindowView => _soundsWindowView;
        public WelcomeBackWindowView WelcomeBackWindowView => _welcomeBackWindowView;
        public ResourcePanelView ResourcePanelView => _resourcePanelView;
        public ShopWindowView ShopWindowView => _shopWindowView;
        public TutorialTasksWindow TutorialTasksWindow => _tutorialTasksWindow;
        public Camera Camera => _mainCamera;
        public Transform Canvas => _canvas;
        public Image TutorialHole => _tutorialHole;
        public GiftChestWindow GiftChestWindow => _giftChestWindow;
        public InfoView InfoView => _infoView;
        public InfoRowsWindow InfoRowsWindowView => _infoRowsView;
        public GoToShopPopup GoToShopPopup => _goToShopPopup;
        public ScrollWorldComponent ScrollWorldComponent => _scrollWorldComponent;


        [Inject]
        public void Init(UIService uiService)
        {
            _uiService = uiService;
        }
        
        public void Awake()
        {
            _uiService.Register(this);
            _recipesWindowView.Close();
        }
    }
}