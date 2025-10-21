using UnityEngine;

namespace Effect
{
    public class ObjectSurface
    {
        public string MaterialType { get; private set; }    
        public ObjectSurface(string type) { MaterialType = type; }  
    }

       
}