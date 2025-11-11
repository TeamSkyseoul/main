using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetRobotOwner", story: "Assign [Hacker] By [Self]", category: "Action", id: "1fa3a867d20030db9d55119a5a0af93f")]
public partial class SetRobotOwnerAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Hacker;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (!Self.Value.TryGetComponent<IHackable>(out var hackable)) return Status.Failure;
        if (hackable.Owner is not Transform owner) return Status.Failure;

        Debug.Log("Success");
        Hacker.Value = owner;
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

