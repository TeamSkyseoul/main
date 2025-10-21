#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Effect.VFXAppearanceComponent))]
[CanEditMultipleObjects]
public class VFXAppearanceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Sync Duration to Effects"))
        {
            foreach (var t in targets)
            {
                var comp = (Effect.VFXAppearanceComponent)t;
                Undo.RecordObject(comp, "Sync VFX Duration");
                comp.SyncDuration();
                EditorUtility.SetDirty(comp);
            }
        }
    }
}
#endif
