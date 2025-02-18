using Settings;
using UnityEngine;

namespace Services.Tasks
{
    public class ActiveTask
    {
        public TaskSettings Settings;
        public int IntCount;
        public float FloatCount;

        public int Count => FloatCount < IntCount ? IntCount : Mathf.FloorToInt(FloatCount);

        public ActiveTask(TaskSettings s, float c)
        {
            Settings = s;
            FloatCount = c;
            IntCount = Mathf.RoundToInt(c);
        }

        public bool IsCompleted()
        {
            var need = Settings.Count == 0 ? 1 : Settings.Count;
            if (Settings.TaskType == TaskType.ResearchAbility)
            {
                need = 1;
            }

            return Count >= need;
        }
    }
}