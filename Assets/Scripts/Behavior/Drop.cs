using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Drop", story: "[Actor] drops", category: "Action/Character", id: "37f6a92c7c32eec8dc7d96a8d341d970")]
public partial class Drop : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor)) return Status.Failure;
        if (actor is IGrab grabber)
        {
            grabber.Drop();
        }
        if (actor is IThrow thrower)
        {
            thrower.Throw(Vector3.up, 0.5f);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

