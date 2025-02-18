using System;
using System.Collections.Generic;
using Services.Boosts;
using Services.Talents;
using Services.Tasks;
using Services.Tutorial;
using Settings;

namespace Services.Analytics
{
    public class AnalyticsService : IDisposable
    {
        private static IYandexAppMetrica Instance => AppMetrica.Instance;
        private TutorialService _tutorialService;
        private BoostService _boostService;
        private TaskService _taskService;
        private SettingsService _settingsService;
        private TalentsService _talentsService;
        
        private Dictionary<string, object> _args = new Dictionary<string, object>();
        
        public AnalyticsService(TutorialService tutorialService,
            BoostService boostService,
            TaskService taskService,
            TalentsService talentsService,
            SettingsService settingsService)
        {
            _tutorialService = tutorialService;
            _boostService = boostService;
            _taskService = taskService;
            _settingsService = settingsService;
            _talentsService = talentsService;

            _tutorialService.OnNextTutorialStep += NextTutorialStepHandler;
            _boostService.OnActivated += BoostActivatedHandler;
            _taskService.OnTaskCompleted += TaskCompletedHandler;
            _talentsService.OnAddedAbility += OnAbilityOpen;
        }

        public void Dispose()
        {
            _tutorialService.OnNextTutorialStep -= NextTutorialStepHandler;
            _boostService.OnActivated -= BoostActivatedHandler;
            _taskService.OnTaskCompleted -= TaskCompletedHandler;
            _talentsService.OnAddedAbility -= OnAbilityOpen;
        }

        private void TaskCompletedHandler(ActiveTask task)
        {
            _args.Clear();
            _args.Add("id", task.Settings.Id);
            _args.Add("branch_id", task.Settings.BranchId);
            _args.Add("type", task.Settings.TaskType.ToString());
            _args.Add("name", task.Settings.Name);
            _args.Add("description", UiUtils.GetTaskDescription(task.Settings, _settingsService));
            
            Instance.ReportEvent("task_completed", _args);
        }
        
        private void NextTutorialStepHandler(int _)
        {
            _args.Clear();
            _args.Add("step_id", _tutorialService.TutorialStep - 1);
            _args.Add("description", UiUtils.GetTutorialDescription(_tutorialService.TutorialStep));

            Instance.ReportEvent("tutorial", _args);
        }

        public void OnCastleUnlocked(int planetId)
        {
            _args.Clear();
            _args.Add("id", planetId);

            Instance.ReportEvent("open_castle", _args);
        }
        public void OnPanelOpen(PanelType type)
        {
            _args.Clear();
            _args.Add("name", type.ToString());

            Instance.ReportEvent("panel_type", _args);
        }
        public void OnAbilityOpen(AbilityType id)
        {
            _args.Clear();
            string abilityName = _settingsService.AbilitiesTree.AbilitiesDict[id].Name;
            _args.Add("name", abilityName);

            Instance.ReportEvent("open_ability", _args);
        }
        public void OnMapObjectTap(bool isCart = false)
        {
            _args.Clear();
            _args.Add("type", isCart? "cart_tap" : "flower_tap");

            Instance.ReportEvent("tap", _args);
        }

        private void BoostActivatedHandler(BoostType boostType, int parameter)
        {
            _args.Clear();
            _args.Add("id", (int)boostType);
            _args.Add("name", boostType.ToString("g"));
            if (boostType == BoostType.CastleBoost)
            {
                _args.Add("castle_id", parameter);
            }

            Instance.ReportEvent("boost", _args);
        }

        public void AdHandler(string result, string description, string source)
        {
            _args.Clear();
            _args.Add("action", result);
            _args.Add("description", description);
            _args.Add("source", source);

            Instance.ReportEvent("ads", _args);
        }
        public void AdFailHandler(string action, string description)
        {
            _args.Clear();
            _args.Add("action", action);
            _args.Add("description", description);

            Instance.ReportEvent("fail_ads", _args);
        }
    }
}