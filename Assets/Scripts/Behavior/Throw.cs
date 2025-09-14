using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Throw", story: "[Actor] throws [Power] at [Target]", category: "Action/Character", id: "8bb7593ee06d42a892f80715f70755be")]
public partial class Throw : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<float> Power;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor)) return Status.Failure;
        if (actor is not IGrab grabbable) return Status.Failure;
        grabbable.Drop();

        if (actor is not IThrow thrower) return Status.Failure;
        var actorPos = Actor.Value.position;
        var targetPos = Target.Value.position;
        var forward = -(actorPos - targetPos).normalized;
        thrower.Throw(forward, Power.Value);
        Debug.DrawLine(Actor.Value.position, Actor.Value.position + forward * Power.Value, Color.red,3f);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
