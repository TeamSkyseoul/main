using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class PolybrushMeshSaver : EditorWindow
{
    private GameObject targetObject;
    private string savePath = "Assets/Polybrush_SavedMeshes";

    [MenuItem("Tools/Polybrush/Save Modified Mesh")]
    public static void ShowWindow()
    {
        GetWindow<PolybrushMeshSaver>("Polybrush Mesh Saver");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save Modified Mesh", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Save Modified Mesh to Asset & Apply to Prefab"))
        {
            if (targetObject == null)
            {
                Debug.LogError("대상을 선택하세요.");
                return;
            }

            var mf = targetObject.GetComponent<MeshFilter>();
            if (mf == null)
            {
                Debug.LogError("MeshFilter가 없습니다.");
                return;
            }

            Mesh mesh = mf.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("Mesh가 없습니다.");
                return;
            }

   
            if (!AssetDatabase.IsValidFolder(savePath))
                AssetDatabase.CreateFolder("Assets", "Polybrush_SavedMeshes");


            string fileName = targetObject.name + "_SavedMesh.asset";
            string path = Path.Combine(savePath, fileName);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

         
            Mesh savedMesh = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(savedMesh, path);
            AssetDatabase.SaveAssets();

         
            mf.sharedMesh = savedMesh;
            EditorUtility.SetDirty(mf);


            GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(targetObject);
            if (prefabRoot != null)
            {
                string prefabPath = AssetDatabase.GetAssetPath(prefabRoot);
                var prefabStage = PrefabUtility.GetPrefabAssetType(prefabRoot);

                if (prefabStage != PrefabAssetType.NotAPrefab)
                {
               
                    GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);
                    var prefabMF = prefabContents.GetComponent<MeshFilter>();
                    if (prefabMF != null)
                    {
                        prefabMF.sharedMesh = savedMesh;
                        EditorUtility.SetDirty(prefabMF);
                    }
                    PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
                    PrefabUtility.UnloadPrefabContents(prefabContents);
                    Debug.Log($"Prefab에 Mesh 적용 완료: {prefabPath}");
                }
            }

            // 씬 저장 상태 갱신
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"저장 완료 및 적용됨: {path}");
        }
    }
}
