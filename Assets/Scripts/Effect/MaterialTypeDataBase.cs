using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effect

{
    [CreateAssetMenu(fileName = "MaterialTypeDatabase", menuName = "ScriptableObject/Material Type Database")]
    public class MaterialTypeDatabase : ScriptableObject
    {
        public List<MaterialTypeData> rows = new List<MaterialTypeData>();
    }
}
