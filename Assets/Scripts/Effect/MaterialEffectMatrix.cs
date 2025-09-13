using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "MaterialEffectMatrix", menuName = "ScriptableObject/Material Effect Matrix")]
    public class MaterialEffectMatrix : ScriptableObject
    {
        [Header("참조할 MaterialTypeDatabase")]
        public MaterialTypeDatabase materialTypeDatabase;

        [Tooltip("행렬 데이터 (i * Count + j 인덱스 사용)")]
        public List<MaterialEffectData> rows = new List<MaterialEffectData>();

        Dictionary<(string, string), MaterialEffectData> cache;

        public void BuildCache()
        {
            cache = new Dictionary<(string, string), MaterialEffectData>();

            if (materialTypeDatabase == null || materialTypeDatabase.rows == null) return;

            int count = materialTypeDatabase.rows.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int index = i * count + j;
                    if (index < 0 || index >= rows.Count) continue;

                    var data = rows[index];
                    if (data == null) continue;

                    string typeA = materialTypeDatabase.rows[i].materialType;
                    string typeB = materialTypeDatabase.rows[j].materialType;

                    var key1 = (typeA, typeB);
                    var key2 = (typeB, typeA);

                    if (!cache.ContainsKey(key1)) cache[key1] = data;
                    if (!cache.ContainsKey(key2)) cache[key2] = data;
                }
            }
        }

        public MaterialEffectData Get(string a, string b)
        {
            if (cache == null || cache.Count == 0)
                BuildCache();

            return cache.TryGetValue((a, b), out var data) ? data : null;
        }

#if UNITY_EDITOR
      
        private void OnValidate()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}
