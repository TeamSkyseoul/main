// SceneExportWindow.cs
// MARKER 제거 + BLOCKER 기반 + 외부 SceneExportSettings 기반 구조
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using System.Linq;
using JetBrains.Annotations;



namespace SceneLoader
{
    public class SceneExportWindow : EditorWindow
    {
         Vector2 scroll;
         List<SceneExportSettings> settingsList = new();
         string exportPath = "Assets/JsonData/BattleMap.json";
         string commonPrefabPath = "Assets/Prefabs/Map";

        [MenuItem("Tools/Scene Export/Export Scene")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneExportWindow>();
            window.titleContent = new GUIContent("Scene Export");
            window.minSize = new Vector2(400, 300);
            window.RefreshExportSettings();
        }



         int cachedTotalCount;
         int cachedAddrCount;
         int cachedPrefabCount;

         const float ItemHeight = 60f;

        void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Scene Export는 Blocker를 제외한 객체를 자동으로 탐색하며,\n" +
                "Addressable / Prefabize 여부를 자동으로 판단합니다.",
                MessageType.Info
            );

            if (GUILayout.Button("Refresh Export List", GUILayout.Height(25)))
                RefreshExportSettings();

            EditorGUILayout.Space(10);


            scroll = EditorGUILayout.BeginScrollView(scroll);

            int count = settingsList.Count;
            float totalHeight = count * ItemHeight;

            Rect visibleRect = new Rect(0, scroll.y, position.width, position.height);
            int firstVisible = Mathf.Max(0, Mathf.FloorToInt(visibleRect.y / ItemHeight) - 2);
            int lastVisible = Mathf.Min(count, Mathf.CeilToInt((visibleRect.y + visibleRect.height) / ItemHeight) + 2);

            GUILayout.Space(firstVisible * ItemHeight);

            for (int i = firstVisible; i < lastVisible; i++)
            {
                if (i < 0 || i >= count) continue;
                DrawSettingsUI(settingsList[i]);
                GUILayout.Space(3);
            }
            cachedAddrCount = settingsList.Count(s => s.MakeAddressable);
            cachedPrefabCount = settingsList.Count(s => s.ForcePrefabize);
            float remainingSpace = Mathf.Max(0, totalHeight - lastVisible * ItemHeight);
            GUILayout.Space(remainingSpace);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

 
            EditorGUILayout.HelpBox(
                $"총 대상: {cachedTotalCount} | Addressable: {cachedAddrCount} | Prefabize: {cachedPrefabCount}",
                MessageType.None
            );

            DrawExportPathSelection();

            bool anyNeedsPrefab = settingsList.Exists(s => PrefabUtilityHelper.NeedsPrefabCreation(s.Target, s));
            if (anyNeedsPrefab)
                DrawCommonPrefabPath();

            EditorGUILayout.Space(8);
            GUI.enabled = !string.IsNullOrEmpty(exportPath);
            if (GUILayout.Button("Export", GUILayout.Height(30)))
                ExportToFile();
            GUI.enabled = true;
        }


        private void DrawSettingsUI(SceneExportSettings s)
        {
            if (s.Target == null) return;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            s.IncludeExport = EditorGUILayout.Toggle(s.IncludeExport, GUILayout.Width(20));
            if (GUILayout.Button(s.Target.name, EditorStyles.label))
            {
                Selection.activeGameObject = s.Target;
                EditorGUIUtility.PingObject(s.Target);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            DrawAddressableOptions(s);
            DrawPrefabizeOptions(s);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

         void DrawAddressableOptions(SceneExportSettings setting)
        {
            bool disabled =
                PrefabUtilityHelper.IsOutermostPrefabInstanceRoot(setting.Target)
                && PrefabUtilityHelper.TryGetAddressFromSource(setting.Target, out _);


            setting.MakeAddressable = !disabled;
           
        }

        void DrawPrefabizeOptions(SceneExportSettings setting)
        {
            bool disabled = PrefabUtilityHelper.HasSourcePrefab(setting.Target);


            setting.ForcePrefabize = !disabled;
        }

        void DrawExportPathSelection()
        {
            EditorGUILayout.LabelField("Export Path", EditorStyles.boldLabel);
            GUIStyle pathStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                normal = { textColor = Color.cyan }
            };

            EditorGUILayout.LabelField(exportPath, pathStyle);

        }

        void DrawCommonPrefabPath()
        {
            EditorGUILayout.LabelField("Common Prefab Save Path", EditorStyles.boldLabel);

       
            GUIStyle pathStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                normal = { textColor = Color.cyan }
            };

            EditorGUILayout.LabelField(commonPrefabPath, pathStyle);
        }

     

        void RefreshExportSettings()
        {
            settingsList.Clear();

            var roots = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var root in roots)
                CollectExportSettingsRecursive(root.transform, false);

           
            cachedTotalCount = settingsList.Count;
         
        }

        void CollectExportSettingsRecursive(Transform t, bool parentExported)
        {
            if (t.GetComponent<SceneExportBlocker>() != null)
                return;

            GameObject obj = t.gameObject;
            bool isOutermost = PrefabUtilityHelper.IsOutermostPrefabInstanceRoot(obj);
            bool exportTarget = PrefabUtilityHelper.IsExportTarget(obj, parentExported);
            bool hasExportedChild = !isOutermost && PrefabUtilityHelper.HasExportedChild(t);

            if (!exportTarget && !hasExportedChild)
                return;

            if (exportTarget)
                settingsList.Add(new SceneExportSettings(obj));

            for (int i = 0; i < t.childCount; i++)
            {
                CollectExportSettingsRecursive(t.GetChild(i), exportTarget);
            }
        }
        void ExportToFile()
        {
            var dataList = new List<SceneObjectData>();
            int nextId = 0;

            HandleExportWithUndo(dataList, ref nextId);
            WriteExportedDataToJsonFile(dataList);
        }
         void HandleExportWithUndo(List<SceneObjectData> dataList, ref int nextId)
        {
            const int MaxObjectPerUndoGroup = 10;

            int undoCounter = 0;
            int groupNumber = 1;
            int group = Undo.GetCurrentGroup();

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Scene Export");

            HashSet<Transform> exportedRoots = new();

            foreach (var setting in settingsList)
            {
                if (!setting.IncludeExport) continue;
                if (exportedRoots.Any(r => setting.Target.transform.IsChildOf(r)))
                    continue;

                if (undoCounter % MaxObjectPerUndoGroup == 0)
                {
                    Undo.CollapseUndoOperations(group);
                    group = Undo.GetCurrentGroup();
                    Undo.IncrementCurrentGroup();
                    Undo.SetCurrentGroupName($"Export Group {groupNumber}");
                    Debug.LogError(groupNumber);

                }

                Undo.RegisterCompleteObjectUndo(setting.Target, "Export Root");
                ExportTransformRecursive(setting.Target.transform, -1, ref nextId, dataList, false, setting);
                exportedRoots.Add(setting.Target.transform);
                undoCounter++;
            }

            Undo.CollapseUndoOperations(group);
        }
         void WriteExportedDataToJsonFile(List<SceneObjectData> dataList)
        {
            string dir = Path.GetDirectoryName(exportPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

       
            var wrapper = new SerializationWrapper<SceneObjectData>(dataList);
            string json = JsonUtility.ToJson(wrapper, true);


            if (File.Exists(exportPath))
                File.Copy(exportPath, exportPath + ".bak", overwrite: true);

            File.WriteAllText(exportPath, json);

          
            Debug.Log($"[SceneExportWindow] Export 완료: {exportPath}");
            AssetDatabase.Refresh();
        }


         void ExportTransformRecursive(Transform t,
            int parentId,
            ref int nextId,
            List<SceneObjectData> resultList
            , bool parentExported,
            SceneExportSettings setting)
        {
            GameObject obj = t.gameObject;
            bool isOutermost = PrefabUtilityHelper.IsOutermostPrefabInstanceRoot(obj);
            bool exportTarget = PrefabUtilityHelper.IsExportTarget(obj, parentExported);
            bool hasChildExport = !isOutermost && PrefabUtilityHelper.HasExportedChild(t);

            if (!exportTarget && !hasChildExport)
                return;

            int currentId = nextId++;
            SceneObjectData data = exportTarget && (isOutermost || (setting != null && setting.ForcePrefabize))
                ? PrefabUtilityHelper.CreatePrefabSceneObjectData(t, currentId, parentId, setting, commonPrefabPath)
                : PrefabUtilityHelper.CreateContainerSceneObjectData(t, currentId, parentId);

            resultList.Add(data);

            int newParentId = currentId;

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                SceneExportSettings childSetting = null;
                if (PrefabUtilityHelper.IsExportTarget(child.gameObject, exportTarget))
                {
                    childSetting = settingsList.FirstOrDefault(s => s.Target == child.gameObject);
                }

                ExportTransformRecursive(child, newParentId, ref nextId, resultList, exportTarget, childSetting);
            }
        }
    }
}