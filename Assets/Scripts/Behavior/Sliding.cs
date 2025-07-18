using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sliding", story: "[Actor] slidings", category: "Action/Character", id: "a13d2cdee769a585b12b30892a790bdf")]
public partial class Sliding : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor) || actor is not ISliding slider) return Status.Failure;

        slider.Slide();
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

