using Services;
using Services.Ads;
using Services.Analytics;
using Services.Craft;
using Services.Boosts;
using Services.ResourceService;
using Services.ResourceVisualizeService;
using Services.ShopService;
using Services.Sounds;
using Services.Tasks;
using Services.Tutorial;
using Services.Updater;
using Services.View;
using Zenject;
using CastleService;
using Services.SimpleTap;
using Services.Talents;
using Services.TapBonus;
using Services.UIResourceAnimator;

namespace Installers
{
    public partial class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InitCore();
            InitMeta();
            InitExtra();
            InitUI();
            InitEffects();
        }

        private void InitCore()
        {
            Container.Bind<SettingsService>().AsSingle().NonLazy();
            
            Container.Bind<UpdateService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<PlayerDataManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<TickService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<MainService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            
            Container.Bind<PlayerResourcesService>().AsSingle().NonLazy();
            
            Container.Bind<ResourceService>().AsSingle().NonLazy();
            Container.Bind<CastlesService>().AsSingle().NonLazy();
            Container.Bind<RecipeService>().AsSingle().NonLazy();
            Container.Bind<CraftService>().AsSingle().NonLazy();
            
        }
        private void InitUI()
        {
            Container.Bind<ViewCastlesService>().AsSingle().NonLazy();
            Container.Bind<UIService>().AsSingle().NonLazy();
            Container.Bind<ResourceVisualizerService>().AsSingle().NonLazy();
        }
        
        private void InitEffects()
        {
            Container.Bind<SoundService>().FromComponentOn(_soundService).AsSingle().NonLazy();
            Container.Bind<EffectsViewService>().FromComponentOn(_effectsService).AsSingle().NonLazy();
            Container.Bind<UIResourceAnimatorService>().AsSingle().NonLazy();
        }
        
        private void InitMeta()
        {
            Container.Bind<TalentsService>().AsSingle().NonLazy();
            Container.Bind<TapBonusService>().AsSingle().NonLazy();
            Container.Bind<TapSecondBonusService>().AsSingle().NonLazy();
            Container.Bind<BoostService>().AsSingle().NonLazy();
            Container.Bind<TaskService>().AsSingle().NonLazy();
            Container.Bind<TutorialHoleController>().AsSingle().NonLazy();
            Container.Bind<TutorialTaskService>().AsSingle().NonLazy();
            Container.Bind<TutorialService>().AsSingle().NonLazy();
            Container.Bind<PurchaseService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }

        private void InitExtra()
        {
            Container.Bind<AdsService>().AsSingle().NonLazy();
            Container.Bind<AdsShowSystem>().AsSingle().NonLazy();
            Container.Bind<AnalyticsService>().AsSingle().NonLazy();
            Container.Bind<ShopService>().AsSingle().NonLazy();
        }
    }
}