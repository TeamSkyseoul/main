using Character;
using NUnit.Framework;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EqualAbnormalState", story: "[Actor] is [AbnormalState]", category: "Variable Conditions", id: "2a80a03ced2716b464276e9e6c58165e")]
public partial class EqualAbnormalState : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<AbnormalStatus> AbnormalState;

    public override bool IsTrue()
    {
        var result = false;
        switch (AbnormalState.Value)
        {
            case global::AbnormalStatus.HitStun:
                if (!Actor.Value.TryGetComponent<IHitStun>(out var hitStun)) break;
                result = hitStun.IsHit;
                break;
            case global::AbnormalStatus.Stun:
                if (!Actor.Value.TryGetComponent<IStun>(out var stun)) break;
                result = stun.IsStun;
                break;
            case global::AbnormalStatus.Die:
                if (!Actor.Value.TryGetComponent<IDeathable>(out var deathable)) break;
                result = deathable.IsDead;
                break;
            default:
                break;
        }

        return result;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
