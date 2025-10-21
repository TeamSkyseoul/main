#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Effect
{

    public class MaterialTypeDropdownAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(MaterialTypeDropdownAttribute))]
    public class MaterialTypeDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // DB Ã£±â
            var db = AssetDatabase.LoadAssetAtPath<MaterialTypeDatabase>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:MaterialTypeDatabase")[0]
                )
            );

            if (db == null || db.rows.Count == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var list = db.rows.ConvertAll(r => r.materialType);
            var current = property.stringValue;

            int index = Mathf.Max(0, list.IndexOf(current));
            index = EditorGUI.Popup(position, label.text, index, list.ToArray());

            property.stringValue = list[index];
        }
    }
}
#endif
