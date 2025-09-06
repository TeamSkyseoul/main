#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;


[ExecuteInEditMode]
public class AdditiveSceneImporter : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset sceneRef;


    private void Awake()
    {
        LoadAdditiveScene();
    }

    void LoadAdditiveScene()
    {
        string path = AssetDatabase.GetAssetPath(sceneRef);
        if (!string.IsNullOrEmpty(path) && !SceneManager.GetSceneByPath(path).isLoaded)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
    }
#endif
}