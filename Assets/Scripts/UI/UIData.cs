using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
   
    [System.Serializable]
    public class NotificationData
    {
        public string key;
        [TextArea]
        public string format;
    }

    [Serializable]
    public class HeightData
    {
        public string ID;
        public float Height;
    }
    [Serializable]
    public class HeightDataBase
    {
        public List<HeightData> entries = new();
    }
}