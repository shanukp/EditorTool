using UnityEngine;
using UI.Tools.Editor.Data;
using UnityEditor;
using System.IO;

namespace  UI.Tools.Editor.Utilities
{
    public class IOUtilities 
    {
        public static ConfigurationData GetConfigData(Object sourceObject)
        {
            var sourcePath = AssetDatabase.GetAssetPath(sourceObject);
            var jsonData = File.ReadAllText(sourcePath);

            ConfigurationData data;

            try
            {
                data = JsonUtility.FromJson<ConfigurationData>(jsonData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("Unable to convert given file to required format - Error mesg {0}", ex.Message));
                data = null;
            }

            return data;
        }
    }
}
