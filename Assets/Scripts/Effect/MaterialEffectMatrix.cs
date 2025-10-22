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
        public List<MaterialEffectData> rows = new();

        [SerializeField, HideInInspector]
        int currentSize = 0;

        public Dictionary<(string, string), MaterialEffectData> cache;

        public MaterialEffectData Get(string a, string b)
        {
            if (cache == null || cache.Count == 0)
                BuildCache();
            return cache.TryGetValue((a, b), out var data) ? data : null;
        }

        public void BuildCache()
        {
            cache = new Dictionary<(string, string), MaterialEffectData>();

            if (materialTypeDatabase == null || materialTypeDatabase.rows == null)
                return;

            EnsureMatrixSize();

            int count = materialTypeDatabase.rows.Count;

            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    int index = GetIndex(i, j, count);
                    if (index < 0 || index >= rows.Count)
                        continue;

                    var data = rows[index];
                    if (data == null)
                        continue;

                    string typeA = materialTypeDatabase.rows[i].materialType;
                    string typeB = materialTypeDatabase.rows[j].materialType;

                    cache[(typeA, typeB)] = data;
                    cache[(typeB, typeA)] = data;
                }
            }
        }

        public void EnsureMatrixSize()
        {
            if (materialTypeDatabase == null)
                return;

            var types = materialTypeDatabase.rows;
            int size = types.Count;

            // 기존 데이터를 임시 보관
            var oldDataMap = new Dictionary<(string, string), MaterialEffectData>();

            int safeOldSize = Mathf.Min(currentSize, types.Count);

            if (rows != null && currentSize > 0)
            {
                for (int i = 0; i < safeOldSize; i++)
                {
                    for (int j = i; j < safeOldSize; j++)
                    {
                        int idx = GetIndex(i, j, currentSize);
                        if (idx < 0 || idx >= rows.Count)
                            continue;
                        if (rows[idx] == null)
                            continue;

                        // 범위 보호
                        if (i >= types.Count || j >= types.Count)
                            continue;

                        string typeA = types[i].materialType;
                        string typeB = types[j].materialType;
                        oldDataMap[(typeA, typeB)] = rows[idx];
                        oldDataMap[(typeB, typeA)] = rows[idx];
                    }
                }
            }

            // 새 크기만큼 다시 생성
            var newRows = new List<MaterialEffectData>();
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    string typeA = types[i].materialType;
                    string typeB = types[j].materialType;

                    if (oldDataMap.TryGetValue((typeA, typeB), out var existing))
                        newRows.Add(existing);
                    else
                        newRows.Add(new MaterialEffectData());
                }
            }

            rows = newRows;
            currentSize = size;
        }

        private int GetIndex(int i, int j, int size)
        {
            if (i > j)
                (i, j) = (j, i);
            return i * size - (i * (i - 1)) / 2 + (j - i);
        }

#if UNITY_EDITOR
        // 에디터에서 안정적 편집을 위한 기능
        public MaterialEffectData GetOrCreate(string a, string b)
        {
            if (cache == null)
                BuildCache();

            if (!cache.TryGetValue((a, b), out var data))
            {
                data = new MaterialEffectData();
                cache[(a, b)] = data;
                cache[(b, a)] = data;
                SaveCacheToRows();
            }

            return data;
        }

        public void SaveCacheToRows()
        {
            if (materialTypeDatabase == null)
                return;

            var types = materialTypeDatabase.rows;
            int size = types.Count;

            var newRows = new List<MaterialEffectData>();

            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    string a = types[i].materialType;
                    string b = types[j].materialType;

                    if (cache != null && cache.TryGetValue((a, b), out var data))
                        newRows.Add(data);
                    else
                        newRows.Add(new MaterialEffectData());
                }
            }

            rows = newRows;
            currentSize = size;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void OnValidate()
        {
            if (materialTypeDatabase == null)
                return;

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                if (materialTypeDatabase == null) return;

                EnsureMatrixSize();
                BuildCache();
                SaveCacheToRows();
            };
        }
#endif
    }
}
