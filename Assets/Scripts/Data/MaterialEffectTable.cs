using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MaterialEffectTable", menuName = "ScriptableObject/Material Effect Table")]
public class MaterialEffectTable : ScriptableObject
{
    public List<MaterialEffectData> rows = new List<MaterialEffectData>();
 //   private Dictionary<(MaterialType, MaterialType), MaterialEffectData> cache;

    //public void BuildCache()
    //{
    //    cache = new Dictionary<(MaterialType, MaterialType), MaterialEffectData>();

    //    foreach (var row in rows)
    //    {
    //        var key1 = (row.TypeA, row.TypeB);
    //        var key2 = (row.TypeB, row.TypeA);

    //        if (!cache.ContainsKey(key1)) cache[key1] = row;
    //        if (!cache.ContainsKey(key2)) cache[key2] = row;
    //    }
    //}

    //public MaterialEffectData Get(MaterialType a, MaterialType b)
    //{
    //    if (cache == null) BuildCache();
    //    return cache.TryGetValue((a, b), out var data) ? data : null;
    //}

  
}