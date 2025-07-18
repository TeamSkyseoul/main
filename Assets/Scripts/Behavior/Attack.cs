using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;
using Battle;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[actor] attacks by [num]", category: "Character/Action", id: "2b893acdb1042987e1eefa135b8ea228")]
public partial class Attack : Action
{
    [SerializeReference] public BlackboardVariable<int> Num;
    [SerializeReference] public BlackboardVariable<GameObject> Actor;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<IActor>(out var actor) || actor is not IAttackable attacker) return Status.Failure;
        attacker.Attack(Num);

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

