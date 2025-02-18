using System;
using System.Collections.Generic;
using System.Text;
using Services;
using Services.Tutorial;
using Settings;
using Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public static class UiUtils
    {
        public static string FormatedTime(int rawSeconds)
        {
            int seconds = rawSeconds % StaticValues.SecondsInMinute;
            int minutes = (rawSeconds - seconds)/ StaticValues.SecondsInMinute;
            int hours = minutes / StaticValues.SecondsInMinute;
            
            var sb = new StringBuilder();

            bool hasHorM = false;
            if (hours > 0)
            {
                sb.Append($"{hours}h ");
                minutes %= StaticValues.SecondsInMinute;
                hasHorM = true;
            }

            if (minutes > 0)
            {
                sb.Append($"{minutes}m ");
                hasHorM = true;
            }

            if (hasHorM && seconds < 1)
            {
                return sb.ToString();
            }
            
            sb.Append($"{seconds}s");
            return sb.ToString();
        }


        public static string GetTaskDescription(TaskSettings settings, SettingsService settingsService)
        {
            string result = default;
            switch (settings.TaskType)
            {
                case TaskType.CollectResource:
                    result = string.Format(settings.Description, settings.Count, StaticNames.Get(settings.ResourceId));
                    break;
                case TaskType.ResearchAbility:
                    result = string.Format(settings.Description,
                        settingsService.AbilitiesTree.AbilitiesDict[settings.Ability].Name);
                    break;
                case TaskType.Unlock:
                case TaskType.Destroy:
                case TaskType.Upgrade:
                    result = string.Format(settings.Description, settings.Count);
                    break;
            }

            return result;
        }
        
        public static string GetTutorialDescription(int step)
        {
            var n = (TutorialStepNames) step;
            return n.ToString();
        }

        public static void UpdateRewards(SettingsService settingsService, List<ResourceDemand> demands, TextMeshProUGUI currency, GameObject[] textIconSpaces, Image[] icons, TextMeshProUGUI[] texts)
        {
            string currencyText = "";
            int index = -1;
            for (int i = 0; i < demands.Count; i++)
            {
                var demand = demands[i];
                if (demand.ResourceId is ResourceNames.Soft or ResourceNames.Hard)
                {
                    currencyText += (i == 1 ? " " : "") +
                                    GetCountableValue(demand.Value,
                                        demand.ResourceId is ResourceNames.Soft ? 0 : 1);
                    continue;
                }
                // i = 2,3
                index = i - 2;
                icons[index].sprite = settingsService.GameResources[demand.ResourceId].Sprite;
                texts[index].text = GetCountableValue(demand.Value);
            }

            currency.text = currencyText;
            for (int j = 0; j < textIconSpaces.Length; j++)
            {
                // index = 0;
                if (index == 0 && j < 2)
                {
                    textIconSpaces[j].SetActive(true);
                    continue;
                }
                if (index == 1 && j < 5)
                {
                    textIconSpaces[j].SetActive(true);
                    continue;
                }
                if (index == 3)
                {
                    textIconSpaces[j].SetActive(true);
                    continue;
                }
                
                textIconSpaces[j].SetActive(false);
            }
        }

        public static string GetCountableValue(float count, int spriteId = -1)
        {
            string icon = spriteId != -1 ? $"<sprite={spriteId}>" : "";
            if (count < Math.Pow(10, 3))
            {
                return $"{icon}{Mathf.FloorToInt(count)}";
            }

            if (count < Math.Pow(10, 6))
            {
                return $"{icon}{Math.Round(count / (Math.Pow(10, 3) * 1f), 2)}K";
            }

            return count < Math.Pow(10, 9) ? $"{icon}{Math.Round(count / (Math.Pow(10, 6) * 1f), 2)}M" : $"{icon}{Math.Round(count / (Math.Pow(10, 9) * 1f), 2)}B";
        }
    }
