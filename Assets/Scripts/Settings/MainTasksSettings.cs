using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "TasksConfig", menuName = "Configs/MainTasksConfig")]
    public class MainTasksSettings : ScriptableObject
    {
        public List<TaskBranchSettings> TaskBranchSettingsList;

        public Dictionary<int, TaskBranchSettings> TaskBranchSettingsMap = new Dictionary<int, TaskBranchSettings>();

        public void Init()
        {
            foreach (var branch in TaskBranchSettingsList)
            {
                TaskBranchSettingsMap[branch.Id] = branch;
                branch.Init();
            }
        }
    }
}