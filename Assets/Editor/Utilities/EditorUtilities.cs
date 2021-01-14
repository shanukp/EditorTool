using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using UI.Tools.Editor.Data;
using System.Linq;

namespace UI.Tools.Editor.Utilities
{

    /// <summary>
    /// Utility classes for the GUI Editor tools
    /// </summary>
    public static class EditorUtilities
    {
        /// <summary>
        /// Used to get assets of a certain type and file extension from entire project which has component T present anywhere in its hierarchy
        /// </summary>
        /// <param name="fileType">The file extention the type uses eg ".asset".</param>
        /// <returns>List of paths of assets which contains component T </returns>
        public static List<string> FindPrefabPathsWithComponent<T>(string fileType , string subdirectory) where T : UnityEngine.Object
        {
            List<string> paths = new List<string>();

            var assets = EditorUtilities.GetAssetsOfType<Object>(fileType, subdirectory);

            for (int i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(assets[i]);
                GameObject gameObject = AssetDatabase.LoadAssetAtPath(path,typeof(GameObject)) as GameObject;

                if (gameObject.GetComponentInChildren<T>() != null)
                {
                    paths.Add(path);
                }
            }

            return paths;
        }

        /// <summary>
        /// Apply Modifications to selected prefabs from the json config file
        /// </summary>
        /// <param name="jsonFile">provided json config file</param>
        /// <param name="selectedPrefabPaths"> prefab paths selected by user to apply the config</param>
        /// <returns> void </returns>
        public static void ApplyModificationsToSelectedPrefabs(Object jsonFile , List<string> selectedPrefabPaths)
        {
            if(selectedPrefabPaths.Count == 0)
            {
                return;
            }

            ConfigurationData data = IOUtilities.GetConfigData(jsonFile);

            if (data != null && data.dataEntries != null)
            {
                for (int i = 0; i < data.dataEntries.Count; ++i)
                {
                    if (selectedPrefabPaths.Count > i)
                    {
                        GameObject prefab = PrefabUtility.LoadPrefabContents(selectedPrefabPaths[i]);

                        Text textComponent = prefab.GetComponentInChildren<Text>();

                        Image imageComponent = prefab.GetComponentInChildren<Image>();

                        if (!string.IsNullOrEmpty(data.dataEntries[i].text))
                        {
                            textComponent.text = data.dataEntries[i].text;
                        }

                        if (!string.IsNullOrEmpty(data.dataEntries[i].color))
                        {
                            if (ColorUtility.TryParseHtmlString(data.dataEntries[i].color, out Color newCol))
                            {
                                textComponent.color = newCol;
                            }
                        }

                        if (!string.IsNullOrEmpty(data.dataEntries[i].imagePath))
                        {
                            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(data.dataEntries[i].imagePath);

                            if (sprite != null)
                            {
                                imageComponent.sprite = sprite;
                            }
                        }

                        PrefabUtility.SaveAsPrefabAsset(prefab, selectedPrefabPaths[i]);
                        PrefabUtility.UnloadPrefabContents(prefab);
                    }
                }
            }
        }


        public static string[] GetAllSubDirectories()
        {
            List<string> directories = new List<string>();

            string[] dirs = Directory.GetDirectories(Application.dataPath);

            foreach(string dir in dirs)
            {

                directories.Add(dir.Split('/').Last());
            }
            
            return directories.ToArray();
        }

        /// <summary>
        /// Used to get assets of a certain type and file extension from entire project
        /// </summary>
        /// <param name="fileExtension">The file extention the type uses eg ".asset".</param>
        /// <returns>An Object array of assets.</returns>
        public static T[] GetAssetsOfType<T>(string fileExtension, string subDirectory) where T : UnityEngine.Object
        {
            List<T> tempObjects = new List<T>();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/" + subDirectory);

            FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

            int i = 0;
            int goFileInfoLength = goFileInfo.Length;

            FileInfo tempGoFileInfo;
            string tempFilePath;
            T tempObj;

            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];

                if (tempGoFileInfo == null)
                {
                    continue;
                }

                tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

                tempObj = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(T)) as T;

                if (tempObj == null)
                {
                    continue;
                }

                tempObjects.Add(tempObj);
            }

            return tempObjects.ToArray();
        }
    }

}