using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour, ITypist
{
    public UnityEvent OnStartEvent;
    public UnityEvent OnEndEvent;
    public event Action<string> OnEndTyping;
    [SerializeField] TextMeshProUGUI ui;
    [Min(0.02f)] public float duration;
    [Min(0f)] public float postDuration;

    public void OnStartTyping(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DoTyping(message));
    }

    IEnumerator DoTyping(string message)
    {
        ui.text = "";
        OnStartEvent.Invoke();
        var wait = new WaitForSeconds(duration);

        if (ui is null)
        {
            OnEndEvent.Invoke();
            OnEndTyping?.Invoke(message);
            yield break;
        }

        for (int i = 0; i < message.Length; i++)
        {
            ui.text += message[i];
            yield return wait;
        }

        yield return new WaitForSeconds(postDuration);
        OnEndEvent.Invoke();
        OnEndTyping?.Invoke(message);
    }
}
