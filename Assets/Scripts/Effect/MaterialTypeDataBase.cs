using Effect;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "MaterialTypeDatabase", menuName = "ScriptableObject/Material Type Database")]
    public class MaterialTypeDatabase : ScriptableObject
    {
        public List<MaterialTypeData> rows = new List<MaterialTypeData>();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MaterialTypeDatabase))]
public class MaterialTypeDatabaseEditor : Editor
{
    SerializedProperty rowsProp;
    bool foldout = true;

    void OnEnable()
    {
        rowsProp = serializedObject.FindProperty("rows");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Material Type Database", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        int newSize = Mathf.Max(0, EditorGUILayout.IntField("Size", rowsProp.arraySize));
        if (newSize != rowsProp.arraySize)
            rowsProp.arraySize = newSize;

        EditorGUILayout.Space(4);

        foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Material Types");

        if (foldout)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < rowsProp.arraySize; i++)
            {
                var element = rowsProp.GetArrayElementAtIndex(i);
                var typeProp = element.FindPropertyRelative("materialType");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Material Type {i}", GUILayout.Width(110));
                typeProp.stringValue = EditorGUILayout.TextField(typeProp.stringValue);

                GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                if (GUILayout.Button("−", GUILayout.Width(22)))
                {
                    rowsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space(6);
            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("+ Add New Material Type"))
            {
                rowsProp.InsertArrayElementAtIndex(rowsProp.arraySize);
                var newElement = rowsProp.GetArrayElementAtIndex(rowsProp.arraySize - 1);
                var typeProp = newElement.FindPropertyRelative("materialType");
                typeProp.stringValue = "NewMaterialType";
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
