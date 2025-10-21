using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "MaterialEffectMatrix", menuName = "ScriptableObject/Material Effect Matrix")]
    public class MaterialEffectMatrix : ScriptableObject
    {
        [Header("참조할 MaterialTypeDatabase")]
        public MaterialTypeDatabase materialTypeDatabase;

        [Tooltip("행렬 데이터 (i, j 조합으로 접근)")]
        public List<MaterialEffectData> rows = new List<MaterialEffectData>();

        [SerializeField, HideInInspector]
        private int currentSize = 0;  

        public Dictionary<(string, string), MaterialEffectData> cache;


        public void EnsureMatrixSize()
        {
            if (materialTypeDatabase == null) return;

            int size = materialTypeDatabase.rows.Count;
            int expected = size * (size + 1) / 2;

           
            var newRows = new List<MaterialEffectData>(expected);

            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    int newIndex = GetIndex(i, j, size);

                 
                    if (rows != null && currentSize > 0 && i < currentSize && j < currentSize)
                    {
                        int oldIndex = GetIndex(i, j, currentSize);
                        if (oldIndex >= 0 && oldIndex < rows.Count)
                        {
                            newRows.Add(rows[oldIndex]);
                            continue;
                        }
                    }

                
                    newRows.Add(new MaterialEffectData());
                }
            }

            rows = newRows;
            currentSize = size;
        }

        public void BuildCache()
        {
            cache = new Dictionary<(string, string), MaterialEffectData>();

            if (materialTypeDatabase == null || materialTypeDatabase.rows == null) return;

            int count = materialTypeDatabase.rows.Count;
            EnsureMatrixSize();

            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    int index = GetIndex(i, j, count);
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

        private int GetIndex(int i, int j, int size)
        {
            if (i > j) (i, j) = (j, i);
            return i * size - (i * (i - 1)) / 2 + (j - i);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureMatrixSize(); 
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
