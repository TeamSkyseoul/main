using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Actor] jumps", category: "Action/Character", id: "ad666e3bb6e7aef58e1d3aae87dd9354")]
public partial class Jump : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;

    protected override Status OnStart()
    {
        if (!(Actor.Value.TryGetComponent<IActor>(out var actor) && actor is IJumpable jumpable))
            return Status.Failure;

        jumpable.Jump();

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

