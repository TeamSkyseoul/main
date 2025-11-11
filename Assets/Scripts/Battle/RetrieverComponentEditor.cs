//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(RetrieverComponent), true)]
//[CanEditMultipleObjects]
//public class RetrieverComponentEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        var syncProp = serializedObject.FindProperty("durationWithEffect");
//        var durationProp = serializedObject.FindProperty("duration");
       
//        EditorGUILayout.PropertyField(syncProp);

//        using (new EditorGUI.DisabledScope(syncProp.hasMultipleDifferentValues || syncProp.boolValue))
//        {
//            EditorGUILayout.PropertyField(durationProp);
//        }

//        DrawPropertiesExcluding(serializedObject, "durationWithEffect", "duration");

//        serializedObject.ApplyModifiedProperties();
//    }
//}
//#endif