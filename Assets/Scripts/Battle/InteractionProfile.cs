using System;
using UnityEngine;

[Serializable]
public class InteractionProfile
{
    [Range(0f, 10f)] public float HoldDuration = 2f;
    [Range(0f, 10f)] public float Range = 3f;
    [Range(0f, 10f)] public float DecaySpeed = 2f;

    public InteractionProfile() { }

    public InteractionProfile(float hold, float range, float decay)
    {
        HoldDuration = hold;
        Range = range;
        DecaySpeed = decay;
    }

    public override string ToString() =>
        $"[InteractionProfile] Hold:{HoldDuration}, Range:{Range}, Decay:{DecaySpeed}";
}
