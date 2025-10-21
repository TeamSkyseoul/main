using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;
using UnityEditor.Rendering.LookDev;
using Battle;


[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "RetrieveObject",
    story: "Retrieve [retrievedObject] to [retriever]",
    category: "Action",
    id: "c5b787e22902433dd574ca8b7cefc0e6")]
public partial class RetrieveObject : Action
{
 
    [SerializeReference] public BlackboardVariable<Transform> retrievedObject;
    [SerializeReference] public BlackboardVariable<Transform> retriever;

    protected override Status OnStart()
    {
        if (retriever.Value.TryGetComponent<IRetriever>(out var retrieve))
            retrieve.Retrieve(retrievedObject.Value);
        return Status.Success;
    }

}
