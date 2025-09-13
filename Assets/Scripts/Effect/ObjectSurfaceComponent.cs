using JetBrains.Annotations;
using UnityEngine;

namespace Effect
{
    public class ObjectSurfaceComponent : MonoBehaviour
    {
        [Header("Material Type")]
        [MaterialTypeDropdown]
        [SerializeField] string materialType;
        public string MaterialType { get { return materialType; } }
    }
}