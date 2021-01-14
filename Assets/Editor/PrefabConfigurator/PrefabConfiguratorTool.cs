using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UI.Tools.Editor.Utilities;

namespace UI.Tools.Editor
{
    /// <summary>
    /// Editor window to allow replacing references to objects across the project
    /// </summary>
    public class PrefabConfiguratorTool : EditorWindow
    {
        /// <summary>
        /// Json file with configuration data to be applied to prefabs
        /// </summary>
        private Object _configurationJsonFile;

        /// <summary>
        /// filtered paths of prefabs which has a TextComponent
        /// </summary>
        private List<string> _assetPaths = new List<string>();

        /// <summary>
        /// Tracks toggle selection state corresponding to each prefab path
        /// </summary>
        private List<bool> _assetPathSelectionStatus = null;

        /// <summary>
        /// Whether all prerequisites for applying configuration are met or not - say whether config file is selected , prefabs are selected etc.
        /// </summary>
        private bool _canApplyConfiguration = false;

        /// <summary>
        /// filter for file extensions where we look for TextComponent
        /// </summary>
        private string _assetTypeToFilter = ".prefab";

        /// <summary>
        /// filter for file search directory where we look for TextComponent
        /// </summary>
        public string[] _directories = null;

        private int _selectedDirectoryIndex = 0;

        private Vector2 _scrollPosition = Vector2.zero;
      
        private string _headerLabelText = null;

        private bool _isDataInitialized = false;

        /// <summary>
        /// keeps a list of paths selected before the apply configuration was triggered
        /// </summary>
        private List<string> _selectedPrefabs = new List<string>();


        [MenuItem("EditorTools/GUI/GUI Prefab Modifier", false, 13)]
        static void Init()
        {
            PrefabConfiguratorTool window = (PrefabConfiguratorTool)EditorWindow.GetWindow(typeof(PrefabConfiguratorTool), true, "GUI Prefab Modifier");
            window.Show();
        }

        void CheckAndInitializeData()
        {
            if (!_isDataInitialized)
            {
                _isDataInitialized = true;
                _directories = EditorUtilities.GetAllSubDirectories();
            }
        }

        void OnGUI()
        {
            CheckAndInitializeData();

            _canApplyConfiguration = false;

            if (_configurationJsonFile != null)
            {
                _canApplyConfiguration = true;
            }

            ShowHeader();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            SetUpHelpBox();

             _configurationJsonFile = EditorGUILayout.ObjectField("Data File (Json)", _configurationJsonFile, typeof(Object), false);

            _assetTypeToFilter = EditorGUILayout.TextField("Asset Type", _assetTypeToFilter);

            EditorGUILayout.LabelField("Select Directory to search");
            _selectedDirectoryIndex = EditorGUILayout.Popup(_selectedDirectoryIndex, _directories);

            EditorGUILayout.Space();

            if (GUILayout.Button("List prefabs that has TextComponent"))
            {
                _assetPaths = EditorUtilities.FindPrefabPathsWithComponent<Text>(_assetTypeToFilter,_directories[_selectedDirectoryIndex]);
                _assetPaths.Sort();
            }

            CheckAndDisplayFilteredAssetsAndUpdateSelectionStatus();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!_canApplyConfiguration);

            if (GUILayout.Button("Apply Configurations from JSON file"))
            {
                if (EditorUtility.DisplayDialog("Apply Configurations from JSON file", "Are you sure you want to apply the changes to selected prefabs? ", "YES", "NO"))
                {
                    UpdateSelectedPrefabPaths();
                    EditorUtilities.ApplyModificationsToSelectedPrefabs(_configurationJsonFile , _selectedPrefabs);
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUILayout.Button("Close"))
            {
                EditorWindow.GetWindow<PrefabConfiguratorTool>().Close();
            }

            EditorGUILayout.Space();
        }

        void ShowHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _headerLabelText = EditorGUIUtility.isProSkin ? "<color=white>ZGUI Reference Replacer</color>" : "ZGUI Reference Replacer";

            EditorGUILayout.LabelField(_headerLabelText, new GUIStyle()
            {
                fontSize = 32,
                alignment = TextAnchor.MiddleCenter
            });

            EditorGUILayout.Space();
        }

        void SetUpHelpBox()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Filters prefabs with TextComponent and then applies the json config to selected prefabs", MessageType.Info);
            EditorGUILayout.Space();
        }

        void CheckAndDisplayFilteredAssetsAndUpdateSelectionStatus()
        {
            if (_assetPaths.Count > 0)
            {
                if (_assetPathSelectionStatus == null)
                {
                    _assetPathSelectionStatus = new List<bool>(new bool[_assetPaths.Count]);
                }

                for (int i = 0; i < _assetPaths.Count; i++)
                {
                    _assetPathSelectionStatus[i] = EditorGUILayout.ToggleLeft(_assetPaths[i], _assetPathSelectionStatus[i]);
                }
            }
        }

        void UpdateSelectedPrefabPaths()
        {
            _selectedPrefabs.Clear();

            for (int i = 0; i < _assetPaths.Count; i++)
            {
                if (_assetPathSelectionStatus[i])
                {
                    _selectedPrefabs.Add(_assetPaths[i]);
                }
            }
        }
    }
}