using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "TutorialBranchSettings", menuName = "Configs/TutorialBranchSettings")]
    public class TutorialBranchSettings : ScriptableObject
    {
        public List<TutorialTaskSettings> Tasks;

        public Dictionary<TutorialTaskType, TutorialTaskSettings> TasksDic =
            new Dictionary<TutorialTaskType, TutorialTaskSettings>();

        public void Init()
        {
            foreach (var task in Tasks)
            {
                TasksDic.Add(task.TaskType, task);
            }
        }
    }
}