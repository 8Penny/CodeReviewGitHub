using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


    namespace Static
    {
        public static class DeleteSaveFunc
        {
        
#if UNITY_EDITOR
            [MenuItem("Tools/Save/Delete")]
#endif
            public static void DeletePlayerData()
            {
                DeletePath(StaticValues.ActivatedDataPath);
                DeletePath(StaticValues.ResourcesDataPath);
                DeletePath(StaticValues.ParametersDataPath);
                DeletePath(StaticValues.UpdatablesDataPath);
                
                PlayerPrefs.DeleteKey(StaticValues.NO_ADS);
            }

            private static void DeletePath(string path)
            {
                if (!File.Exists(path))
                {
                    return;
                }
                File.Delete(path);
            }
        }
    }
