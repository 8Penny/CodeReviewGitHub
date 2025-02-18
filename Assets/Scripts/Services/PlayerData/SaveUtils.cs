using System.IO;

namespace Services.PlayerData
{
    public static class SaveUtils
    {
        public static bool TryLoad(out System.Object data, string fullPath)
        {
            data = null;
            if (!File.Exists(fullPath))
            {
                return false;
            }
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            data = formatter.Deserialize(stream);
            stream.Close();
            return true;
        }

        public static void Save(System.Object saveObject, string path)
        {
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(stream, saveObject);
            stream.Close();
        }
    }
}