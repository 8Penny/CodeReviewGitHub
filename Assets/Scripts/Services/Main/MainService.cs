using CastleService;
using UnityEngine;
using Zenject;
using Services.ResourceVisualizeService;
using Services.Ads;
using Services.Craft;
using Services.Tasks;
using Services.Tutorial;

namespace Services
{
    public class MainService : MonoBehaviour
    {
        private PlayerResourcesService _resourcesService;
        private PlayerDataManager _dataManager;
        private CastlesService _castlesService;
        private TickService _tickService;
        private TaskService _taskService;
        private CraftService _craftService;
        private AdsService _adsService;
        private TutorialService _tutorialService;

        
        [Inject]
        public void Init(PlayerDataManager dataManager,
            PlayerResourcesService resourcesService,
            TaskService taskService, TickService tickService,
            CraftService craftService, AdsService adsService,
            ResourceVisualizerService visualizerService,
            CastlesService castlesService, TutorialService tutorialService)
        {
            _resourcesService = resourcesService;
            _taskService = taskService;
            _craftService = craftService;
            _adsService = adsService;
            _dataManager = dataManager;
            _tickService = tickService;
            _castlesService = castlesService;
            _tutorialService = tutorialService;
        }

        public void Awake()
        {
            LaunchAllServices();
        }

        private void LaunchAllServices()
        {
            _dataManager.LoadPlayerData();
            _tickService.PreInitStep();
            _taskService.Init();
            _castlesService.UpdateCastles();

            _craftService.UpdateBenches();
            _resourcesService.Init();

            _tickService.AddNewEntities();
            _tickService.Init();
            
            _adsService.Init();

        }
        
        public void Start()
        {
            _castlesService.UpdateViews();
        }
    }
}