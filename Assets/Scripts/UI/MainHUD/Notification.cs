using System.Collections.Generic;

using UnityEngine;


namespace GameUI
{

    public class Notification : UIWidget
    {
        [Header("Content Size Fitter 오브젝트")]
        [SerializeField] Transform content;
        [Header("알림 메시지 Data")]
        [SerializeField] NotificationDatabase notifyData;
        [SerializeField] GameObject textObject;
        [Range(0, 10)]
        [SerializeField] float disappearTime;

        Queue<NotifyText> notifyTexts = new();
        private void OnDisable() { ClearAllSlot(); }
        public void ShowMessage(string type, string target = "")
        {
            string message = string.IsNullOrEmpty(target)
                ? notifyData.GetMessage(type)
                : notifyData.GetMessage(type, target);
            Debug.Log($"[ShowMessage] 메시지 설정: {message},");
            if (!string.IsNullOrEmpty(message))
                CreateMessage(message);
        }

        private void CreateMessage(string message)
        {
            GameObject obj = Instantiate(textObject, content);
            NotifyText text = obj.GetComponent<NotifyText>();

            if (text == null)
            {
                Debug.Log("텍스트가 없음");
                return;
            }

            text.OnExpired += HandleExpired;
            Debug.Log($"[CreateMassage] 메시지 설정: {message},");
            text.SetMessage(message, disappearTime);
            notifyTexts.Enqueue(text);
        }


        private void HandleExpired(NotifyText expiredText)
        {
            if (notifyTexts.Count > 0 && notifyTexts.Peek() == expiredText)
                notifyTexts.Dequeue();

            Destroy(expiredText.gameObject);
        }


        public void ClearAllSlot()
        {
            while (notifyTexts.Count > 0)
            {
                var text = notifyTexts.Dequeue();
                if (text != null)
                    Destroy(text.gameObject);
            }
        }
    }
}
