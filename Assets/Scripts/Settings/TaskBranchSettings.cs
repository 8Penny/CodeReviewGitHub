using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "TaskBranchConfig", menuName = "Configs/TaskBranch")]
    public class TaskBranchSettings : ScriptableObject
    {
        public int Id;
        public bool IsOpen;
        public int BranchIdOpenCondition;
        public int TaskIdOpenCondition;
        public List<TaskSettings> Tasks;

        public Dictionary<int, TaskSettings> TasksMap = new Dictionary<int, TaskSettings>();
        public void Init()
        {
            foreach (var t in Tasks)
            {
                TasksMap[t.Id] = t;
                t.BranchId = Id;
            }
        }
    }
}