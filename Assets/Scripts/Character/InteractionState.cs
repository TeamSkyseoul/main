using System;
using UnityEngine;


[Serializable]
public class InteractionState
{
    public float HeldTime { get; private set; }
    public bool IsActive { get; private set; }

    public float Progress => Mathf.Clamp01(HeldTime / Mathf.Max(0.01f, requiredTime));

    float requiredTime;

    public void Begin()
    {
        IsActive = true;
        HeldTime = 0f;
    }

    public void Tick(float deltaTime, Action<float> onProgress)
    {
        if (!IsActive) return;

        HeldTime += deltaTime;
        onProgress?.Invoke(Progress);
    }

    public void TickDecay(float deltaTime, float decaySpeed, Action<float> onProgress)
    {
        if (IsActive || HeldTime <= 0f) return;

        HeldTime = Mathf.Max(0f, HeldTime - deltaTime * decaySpeed);
        onProgress?.Invoke(Progress);
    }

    public bool IsCompleted(float requiredTime)
    {
        this.requiredTime = requiredTime;
        return HeldTime >= this.requiredTime;
    }

    public void Cancel() => IsActive = false;

    public void Reset(Action<float> onProgress)
    {
        IsActive = false;
        HeldTime = 0f;
        onProgress?.Invoke(0f);
    }
}
