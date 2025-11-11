using Character;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EqualRobotSubState", story: "[Actor] is [RobotSubState]", category: "Conditions", id: "aae0090978cbebeac58db96a2d690c58")]
public partial class EqualRobotSubState : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<RobotSubStatus> RobotSubState;

    public override bool IsTrue()
    {
        if (!Actor.Value.TryGetComponent<IHackable>(out var wakeable)) return false;

        if (RobotSubState.Value == RobotSubStatus.Sleep) return !wakeable.IsWake;
        else return wakeable.IsWake;

    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
