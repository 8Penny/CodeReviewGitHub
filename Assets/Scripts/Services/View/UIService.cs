using System;
using Services.Sounds;
using Services.Tutorial;
using UIBasics.Views;
using UIBasics.Views.AbilityPanel;

namespace Services
{
    public class UIService : IDisposable
    {
        private MainViewsContainer _viewsContainer;
        private SoundService _soundService;
        private TutorialService _tutorialService;

        private bool _isPanelActive;
        private bool _isWindowActive;

        public bool ChoosingCastle => _viewsContainer != null && _viewsContainer.ChooseCastleBoost.gameObject.activeSelf;
        public MainViewsContainer Views => _viewsContainer;
        public Action<PanelType> OnPanelChanged;
        public Action OnViewsRegistered;
        public Action<bool> OnWindowChangedVisibility;
        public Action<bool> OnChooseCastleStateUpdated;

        public bool IsPanelActive => _isPanelActive;
        public bool IsWindowActive => _isWindowActive;

        public UIService(SoundService soundService)
        {
            _soundService = soundService;
            OnWindowChangedVisibility += UpdateActiveValue;
        }

        public void Dispose()
        {
            OnWindowChangedVisibility -= UpdateActiveValue;
        }

        public void Register(MainViewsContainer container)
        {
            _viewsContainer = container;
            OnViewsRegistered?.Invoke();
        }

        public void Register(TutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        private void UpdateActiveValue(bool isWindowActive)
        {
            _isWindowActive = isWindowActive;
        }

        public void ShowCastlePanel(Castle castle)
        {
            _viewsContainer.CastlePanelView.SetValue(castle);
            _viewsContainer.DataPanelView.OpenPanel(PanelType.Castles);
            _viewsContainer.DataPanelView.SetHeader(castle.Settings.Name);
            _isPanelActive = true;
            OnPanelChanged?.Invoke(PanelType.Castles);
        }
        
        public void OnPanelButtonClicked(PanelType panelType)
        {
            _viewsContainer.DataPanelView.OpenPanel(panelType);
            _isPanelActive = true;
            OnPanelChanged?.Invoke(panelType);
        }

        public void ShowAbilityWindow(AbilityView ability)
        {
            _soundService.PlayClick();
            _viewsContainer.AbilityWindowView.SetAbility(ability);
            _viewsContainer.AbilityWindowView.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void ShowRecipesWindow(int benchId)
        {
            _viewsContainer.RecipesWindowView.Show(benchId);
            OnWindowChangedVisibility?.Invoke(true);
        }
        public void ShowSoundsWindow()
        {
            _soundService.PlayClick();
            _viewsContainer.SoundsWindowView.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }
        
        public void ShowShopWindow()
        {
            _soundService.PlayClick();
            _viewsContainer.ShopWindowView.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void ShowWelcomeBack(ResourcesHolder resourcesHolder, int seconds)
        {
            _soundService.PlayClick();
            
            _viewsContainer.AbilityWindowView.Close();
            _viewsContainer.RecipesWindowView.Close();
            _viewsContainer.SoundsWindowView.Close();
            _viewsContainer.ShopWindowView.Close();
            _viewsContainer.TutorialTasksWindow.Close();
            _viewsContainer.InfoRowsWindowView.Close();
            _viewsContainer.GoToShopPopup.Close();
            
            _viewsContainer.WelcomeBackWindowView.SetData(resourcesHolder, seconds);
            _viewsContainer.WelcomeBackWindowView.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void ShowTutorialTaskWindow()
        {
            _soundService.PlayClick();
            _viewsContainer.TutorialTasksWindow.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void ShowInfoWindow()
        {
            _soundService.PlayClick();
            _viewsContainer.InfoRowsWindowView.Show();
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void CloseWindow(bool withSound = true)
        {
            if (withSound)
            {
                _soundService.PlayClick();
            }
            _viewsContainer.AbilityWindowView.Close();
            _viewsContainer.RecipesWindowView.Close();
            _viewsContainer.SoundsWindowView.Close();
            _viewsContainer.WelcomeBackWindowView.Close();
            _viewsContainer.ShopWindowView.Close();
            _viewsContainer.TutorialTasksWindow.Close();
            _viewsContainer.InfoRowsWindowView.Close();
            _viewsContainer.GoToShopPopup.Close();
            OnWindowChangedVisibility?.Invoke(false);
        }

        public void OpenGoToShopPopup(bool isSoft = true)
        {
            if (!_tutorialService.IsComplete)
            {
                return;
            }
            _viewsContainer.GoToShopPopup.Show(isSoft);
            OnWindowChangedVisibility?.Invoke(true);
        }

        public void CloseMainPanel()
        {
            _soundService.PlayClick();
            _viewsContainer.DataPanelView.UpdateVisibility(false);
            _isPanelActive = false;
            
            OnPanelChanged?.Invoke(PanelType.None);
        }

        public void OnChooseCastleUpdated(bool isVisible)
        {
            OnChooseCastleStateUpdated?.Invoke(isVisible);
        }
    }
}