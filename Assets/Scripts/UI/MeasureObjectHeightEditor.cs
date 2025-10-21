using GameUI;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MeasureObjectHeightEditor : EditorWindow
{
    GameObject targetPrefab;
    GameObject prevPrefab;
    Vector3 lastAnchorPos;
    float defaultAnchorOffset = 0.5f;
    bool isMeasured = false;
    const string Find_Asset_Path = "t:TextAsset HeightDatabase";
    [MenuItem("Tools/GameUI/Measure Height")]
    static void Init()
    {
        GetWindow<MeasureObjectHeightEditor>("WorldUI Height Tool");
    }

    void OnGUI()
    {
        DrawPrepareField();

        if (targetPrefab == null) return;

        if(targetPrefab!=prevPrefab)
        {
            isMeasured = false;
            prevPrefab = targetPrefab;
        }

        if (GUILayout.Button("키 측정하기")) { MeasureHeight(targetPrefab); }
        if (isMeasured) 
        {
            DrawResultField();
            DrawSaveField();
        }
    }

    void DrawPrepareField()
    {
        GUILayout.Label("WorldUI Height Tool", EditorStyles.boldLabel);

        targetPrefab = (GameObject)EditorGUILayout.ObjectField
            (
            "Target Prefab", targetPrefab, typeof(GameObject), false
            );

        defaultAnchorOffset = EditorGUILayout.FloatField("고정 Anchor 오프셋", defaultAnchorOffset);

    }
    void DrawResultField()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("프리팹 이름 : ", targetPrefab.name);
        EditorGUILayout.LabelField("Anchor 포지션 값 : ", lastAnchorPos.ToString("F2"));
    }
    void MeasureHeight(GameObject prefab)
    {
        float height = CalculateHeight(prefab);
        lastAnchorPos = GetSuggestedAnchor(height);
        isMeasured = true;
        Debug.Log($"[Measure Height] Prefab: {prefab.name}, Height: {height:F2}, Suggested Anchor LocalPos: {lastAnchorPos}");
    }

    float CalculateHeight(GameObject prefab)
    {
        if (TryGetHeightFromAnimator(prefab, out float height)) return height;
        if (TryGetHeightFromRenderer(prefab, out height)) return height;
        if (TryGetHeightFromCollider(prefab, out height)) return height;

        return GetFallbackHeight();
    }

    bool TryGetHeightFromAnimator(GameObject prefab, out float height)
    {
        height = 0f;
        var animator = prefab.GetComponentInChildren<Animator>();
        if (animator != null && animator.isHuman)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            if (head != null)
            {
                height = head.position.y - prefab.transform.position.y;
                return height > 0f;
            }
        }
        return false;
    }

    bool TryGetHeightFromRenderer(GameObject prefab, out float height)
    {
        height = 0f;
        var renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            height = bounds.size.y;
            return height > 0f;
        }
        return false;
    }

    bool TryGetHeightFromCollider(GameObject prefab, out float height)
    {
        height = 0f;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = Vector3.zero;

        var colliders = instance.GetComponentsInChildren<Collider>();
        if (colliders.Length > 0)
        {
            Bounds bounds = colliders[0].bounds;
            foreach (var c in colliders)
                bounds.Encapsulate(c.bounds);

            height = bounds.size.y;
        }

        DestroyImmediate(instance);

        return height > 0f;
    }
    float GetFallbackHeight() { return 2f; }
    Vector3 GetSuggestedAnchor(float height) { return new Vector3(0, height + defaultAnchorOffset, 0); }

    #region SaveData
    string FindAsset(string filter)
    {
        string[] guids = AssetDatabase.FindAssets(filter);
        if (guids.Length > 0)
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        return null;
    }

    HeightDataBase LoadDatabase(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return new HeightDataBase();

        string json = File.ReadAllText(path);
        var db = JsonUtility.FromJson<HeightDataBase>(json);
        return db ?? new HeightDataBase();
    }

    void SaveDatabase(string path, HeightDataBase db)
    {
        string newJson = JsonUtility.ToJson(db, true);
        File.WriteAllText(path, newJson);
        AssetDatabase.Refresh();
    }

    void AddOrUpdateEntry(HeightDataBase db, string id, float height)
    {
        var entry = db.entries.FirstOrDefault(e => e.ID == id);
        if (entry == null)
        {
            entry = new HeightData { ID = id, Height = height };
            db.entries.Add(entry);
            Debug.Log($"[HeightDB] 새 데이터 추가: {targetPrefab.name} (ID={id})");
        }
        else
        {
            entry.Height = height;
            Debug.Log($"[HeightDB] 데이터 갱신: {targetPrefab.name} (ID={id})");
        }
    }

    void DrawSaveField()
    {
        if (GUILayout.Button("데이터 저장하기"))
        {
            if (targetPrefab == null || !isMeasured) return;

            string id = targetPrefab.name;
            float height = lastAnchorPos.y;

            string path = FindAsset(Find_Asset_Path);

            if (string.IsNullOrEmpty(path))
            { 
                path = "Assets/HeightDatabase.json";
                File.WriteAllText(path, JsonUtility.ToJson(new HeightDataBase(), true));
                AssetDatabase.Refresh();
            }
            
            var db = LoadDatabase(path);
            AddOrUpdateEntry(db,id , height);
            SaveDatabase(path, db);
        }
    }
    #endregion
}
