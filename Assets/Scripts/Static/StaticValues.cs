using UnityEngine;

namespace Static
{
    public static class StaticValues
    {
        public static string UpdatablesDataPath => Application.persistentDataPath + "/updatables.save";
        public static string ResourcesDataPath => Application.persistentDataPath + "/resources.save";
        public static string ActivatedDataPath => Application.persistentDataPath + "/activated.save";
        public static string ParametersDataPath => Application.persistentDataPath + "/parameters.save";
        
        public static string NO_ADS => "NoAds";

        public static int SaveVersion = 1;
        public static float GroundYPos = 3200;
        public static float BumMaxSumLevel = 25;
        public static float DistanceDivider = 7.3f;
        public static int StartSoftValue = 300;
        public static int StartHardValue = 30;
        public static int SecondsInMinute = 60;
        public static bool IsDevelop = false;
        public static bool IsDevelopADS = false;
        
        public static Color InactiveTextColor = new Color(1, 1, 1, 0.7f); 
    }
}