using Battle;
using Character;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EqualBasicStatus", story: "[Actor] is [status]", category: "Variable Conditions", id: "67a0533ef321ad4d004a5cc0f82b2e46")]
public partial class EqualBasicStatus : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<BasicStatus> Status;

    public override bool IsTrue()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor)) return false;

        switch (Status.Value)
        {
            case BasicStatus.IsGrounded:
                { if (actor is IGroundCheckable groundCheckable) return groundCheckable.IsGrounded; else return true; }
            case BasicStatus.IsAiring:
                { if (actor is IGroundCheckable groundCheckable) return !groundCheckable.IsGrounded; else return false; }
            default:
                return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
