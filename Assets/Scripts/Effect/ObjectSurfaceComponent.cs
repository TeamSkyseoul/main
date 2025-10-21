using JetBrains.Annotations;
using UnityEngine;

namespace Effect
{
    public class ObjectSurfaceComponent : MonoBehaviour
    {
        ObjectSurface surface;
        [Header("Material Type")]
        [MaterialTypeDropdown]
        [SerializeField] string materialType;

        public string MaterialType => materialType;
        private void Start()
        {
            if(materialType != null) surface =new ObjectSurface(materialType);
        }
    }
}