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
}