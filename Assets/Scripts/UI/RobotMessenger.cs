using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITypist
{
    public void OnStartTyping(string messages);
    public event Action<string> OnEndTyping;
}

public class RobotMessenger : MonoBehaviour
{
    bool IsTyping;
    ITypist typist;
    ITypist Typist
    {
        get
        {
            if (typist is null)
            {
                typist = GetComponent<ITypist>();
                typist.OnEndTyping += OnEndTyping;
            }
            return typist;
        }
    }
    readonly Queue<string> messageQueue = new();

    static RobotMessenger instance;
    static RobotMessenger Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindFirstObjectByType<RobotMessenger>();
            if (instance == null) instance = new GameObject(nameof(RobotMessenger)).AddComponent<RobotMessenger>();
            return instance;
        }
    }

    public void PrintMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (Typist is null) return;

        messageQueue.Enqueue(message);
        if (!IsTyping) Typist.OnStartTyping(messageQueue.Dequeue());
        IsTyping = true;
    }
    void OnEndTyping(string message)
    {
        if (messageQueue.Count == 0)
        {
            IsTyping = false;
            return;
        }
        Typist.OnStartTyping(messageQueue.Dequeue());
    }
}