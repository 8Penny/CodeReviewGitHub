using System;
using System.Collections.Generic;
using Services;
using Settings;


[Serializable]
public class ActivatedData
{
    public List<TaskData> Tasks;
    public List<AbilityType> UnlockedAbilities;
    public List<TutorialTaskData> TutorialTasks;
    public long[] Boosts;

    public ActivatedData()
    {
        UnlockedAbilities = new List<AbilityType>();
        Tasks = new List<TaskData>();
        TutorialTasks = new List<TutorialTaskData>();
        Boosts = new long[3];
    }
}
