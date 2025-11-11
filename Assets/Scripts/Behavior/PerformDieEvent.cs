using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PerformDieEvent", story: "Perform [self] DieEvent", category: "Action", id: "d954f357e6754674cb5e02b13652cf7c")]
public partial class PerformDieEvent : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (!Self.Value.TryGetComponent<IExplode>(out var explode)) return Status.Failure;

        explode.Explosion();
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

