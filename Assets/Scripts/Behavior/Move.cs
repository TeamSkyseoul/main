using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move", story: "[Actor] moves with [direction] and [power]", category: "Character/Action", id: "1407a7ac9ad91fd9bc2146d9c11008b3")]
public partial class Move : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<Vector3> Direction;
    [SerializeReference] public BlackboardVariable<float> Power;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor) || actor is not IMovable movable) return Status.Failure;

        movable.Move(Direction.Value, Power.Value);
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

