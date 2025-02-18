namespace Settings
{
    public static class SettingsHelper
    {
        public static bool ShowAbility(TaskType t)
        {
            return t == TaskType.ResearchAbility;
        }
    }
}