using System;
using Services.PlayerData;
using Services.Tutorial;
using Static;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
     private MainPlayerSaveData _mainPlayerSaveData;
     private bool _isFirstInit;
     private bool _isLoaded;
     
     public Action OnSaveLoaded;
     public Action OnPreSaveStep;

     public bool IsFirstInit => _isFirstInit;
     public bool IsLoaded => _isLoaded;
     public UpdatableEntities Updatables => _mainPlayerSaveData?.Updatables;
     public ActivatedData Activated => _mainPlayerSaveData?.Activated;
     public ResourcesHolder PlayerResources => _mainPlayerSaveData?.MainResourcesHolder;
     public SimpleParameters Parameters => _mainPlayerSaveData?.SimpleParameters;
     
     public void LoadPlayerData()
     {
         _mainPlayerSaveData = new MainPlayerSaveData();
         _isFirstInit = true;

         int saveVersion = PlayerPrefs.GetInt("SaveVersion");
         if (saveVersion != StaticValues.SaveVersion)
         {
             PlayerPrefs.SetInt("SaveVersion", StaticValues.SaveVersion);
             DeleteSaveFunc.DeletePlayerData();
         }
         
         if (SaveUtils.TryLoad(out var resources, StaticValues.ResourcesDataPath))
         {
             _mainPlayerSaveData.MainResourcesHolder = (ResourcesHolder) resources;
             _isFirstInit = false;

             if (StaticValues.IsDevelop)
             {
                 foreach (ResourceNames i in Enum.GetValues(typeof(ResourceNames)))
                 {
                    _mainPlayerSaveData.MainResourcesHolder.AddResource(i, 50);
                 }
             }
         }
         else
         {
             _mainPlayerSaveData.MainResourcesHolder = new ResourcesHolder();
         }
         
         if (SaveUtils.TryLoad(out var updatables, StaticValues.UpdatablesDataPath))
         {
             _mainPlayerSaveData.Updatables = (UpdatableEntities) updatables;
         }
         else
         {
             _mainPlayerSaveData.Updatables = new UpdatableEntities();
         }
         
         if (SaveUtils.TryLoad(out var activated, StaticValues.ActivatedDataPath))
         {
             _mainPlayerSaveData.Activated = (ActivatedData) activated;
         }
         else
         {
             _mainPlayerSaveData.Activated = new ActivatedData();
         }
         
         if (SaveUtils.TryLoad(out var parameters, StaticValues.ParametersDataPath))
         {
             _mainPlayerSaveData.SimpleParameters = (SimpleParameters) parameters;

         }
         else
         {
             _mainPlayerSaveData.SimpleParameters = new SimpleParameters();
         }

         FillStartValues();
         _isLoaded = true;
         OnSaveLoaded?.Invoke();
     }

     private void FillStartValues()
     {
         if (!_isFirstInit)
         {
             int current = _mainPlayerSaveData.SimpleParameters.LastTutorialStep;
             if (current == (int) TutorialStepNames.ActivatedBoost)
             {
                 _mainPlayerSaveData.SimpleParameters.LastTutorialStep = (int) TutorialStepNames.TakenEmeralds;
             } else if (current >= (int)TutorialStepNames.StartedAbilityShowing && current < (int)TutorialStepNames.Ability4Shown)
             {
                 _mainPlayerSaveData.SimpleParameters.LastTutorialStep = (int) TutorialStepNames.SellAndEarnTasks;
             }
             return;
         }
         int startMoney = StaticValues.IsDevelop? 1000 : StaticValues.StartSoftValue;

         _mainPlayerSaveData.MainResourcesHolder.AddResource(ResourceNames.Soft, startMoney);
         _mainPlayerSaveData.MainResourcesHolder.AddResource(ResourceNames.Hard, StaticValues.StartHardValue);
     }

     private void SaveAll()
     {
         SaveUtils.Save(_mainPlayerSaveData.Activated, StaticValues.ActivatedDataPath);
         SaveUtils.Save(_mainPlayerSaveData.MainResourcesHolder, StaticValues.ResourcesDataPath);
         SaveUtils.Save(_mainPlayerSaveData.Updatables, StaticValues.UpdatablesDataPath);
         SaveUtils.Save(_mainPlayerSaveData.SimpleParameters, StaticValues.ParametersDataPath);
     }

     public void StoreData()
     {
         OnPreSaveStep?.Invoke();
         SaveAll();
     }
     public void OnApplicationQuit()
     {
         StoreData();
     }
}
