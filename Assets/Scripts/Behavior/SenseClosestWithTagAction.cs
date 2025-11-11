using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SenseClosestWithTagAction",
    story: "Sense [Target] closest to [Agent] with [TagAction]", 
    category: "Action",
    id: "e19f20597e8a5fc783cf08d7fdab9f17")]
public partial class SenseClosestWithTagAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<string> TagAction;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent provided.");
            return Status.Failure;
        }

        Vector3 agentPosition = Agent.Value.transform.position;

        GameObject[] targets = GameObject.FindGameObjectsWithTag(TagAction.Value);
        float closestDistance = Mathf.Infinity;
        GameObject closestGameObject = null;

        int index = 0;
        for(int i=0; i<targets.Length; i++)
        {
            index = i;
            float distanceSq = Vector3.SqrMagnitude(agentPosition - targets[index].transform.position);
            if(closestGameObject==null||distanceSq<closestDistance)
            {
                closestDistance = distanceSq;
                closestGameObject = targets[index];
            }
        }

        Target.Value = closestGameObject;
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

