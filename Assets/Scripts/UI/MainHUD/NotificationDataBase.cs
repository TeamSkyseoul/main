using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    [CreateAssetMenu(fileName = "NotificationDatabase", menuName = "GameUI/NotificationDatabase")]
    public class NotificationDatabase : ScriptableObject
    {
        public const int MaxCount = 30;

        [SerializeField, HideInInspector]
        private List<NotificationData> notifications = new List<NotificationData>();

        private Dictionary<string, string> cache;

        public IReadOnlyList<NotificationData> Notifications => notifications;

        private void OnEnable()
        {
            RebuildCache();
        }

        public void RebuildCache()
        {
            cache = new Dictionary<string, string>();
            foreach (var data in notifications)
            {
                if (data != null && !string.IsNullOrEmpty(data.key))
                {
                    if (!cache.ContainsKey(data.key))
                        cache[data.key] = data.format;
                    else
                        Debug.LogWarning($"[NotificationDatabase] 중복된 key 발견: {data.key}");
                }
            }
        }

        public string GetMessage(string key, params object[] args)
        {
            if (cache == null) RebuildCache();
            if (!cache.TryGetValue(key, out var format))
                return string.Empty;

            return args.Length > 0 ? string.Format(format, args) : format;
        }

#if UNITY_EDITOR
        public void EditorRebuildCache() => RebuildCache();

        public void EditorAdd(NotificationData data)
        {
            if (notifications.Count >= MaxCount)
            {
                Debug.LogWarning($"[NotificationDatabase] 최대 {MaxCount}개까지만 추가할 수 있습니다.");
                return;
            }
            notifications.Add(data);
        }

        public void EditorRemoveAt(int index)
        {
            if (index >= 0 && index < notifications.Count)
                notifications.RemoveAt(index);
        }

        public void EditorReplaceAt(int index, NotificationData data)
        {
            if (index >= 0 && index < notifications.Count)
                notifications[index] = data;
        }
#endif
    }
}
