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
    public bool enableOnAwake;

    private void Awake()
    {
        LoadAdditiveScene();
    }

    void LoadAdditiveScene()
    {
        if (UnityEditor.EditorApplication.isPlaying) return;
        string path = AssetDatabase.GetAssetPath(sceneRef);
        if (!string.IsNullOrEmpty(path) && !SceneManager.GetSceneByPath(path).isLoaded)
        {
            var sceneMode = enableOnAwake ? OpenSceneMode.Additive : OpenSceneMode.AdditiveWithoutLoading;
            var scene = EditorSceneManager.OpenScene(path, sceneMode);
        }
    }
#endif
}