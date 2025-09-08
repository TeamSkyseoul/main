using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NotifyText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifyMessage;
    
    public Action<NotifyText> OnExpired;

    public void SetMessage(string message, float durationTime)
    {
        notifyMessage.text = message;
        StartCoroutine(LifeTime(durationTime));
    }
    IEnumerator LifeTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        //TODO: FADE ANIMATION 

        OnExpired?.Invoke(this);
    }
}
