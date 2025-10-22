using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Get Wander Position",
    story: "Set [WanderPos]  around [Target] within [PatrolRadius]",
    category: "Action",
    id: "da5f64ff7372e60289fd3bc1e3109381"
)]
public partial class SetWanderPos : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> WanderPos;
    [SerializeReference] public BlackboardVariable<float> PatrolRadius;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        if (Target.Value == null)
            return Status.Failure;

        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * PatrolRadius.Value;
        Vector3 offset = new Vector3(randomOffset.x, 0, randomOffset.y);
        WanderPos.Value = Target.Value.position + offset;
        return Status.Success;
    }
}
