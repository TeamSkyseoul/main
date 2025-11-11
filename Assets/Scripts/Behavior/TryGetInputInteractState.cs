using Character;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TryGetInputInteract", story: "TryGet InteractKey out [InteractState]", category: "Conditions", id: "f0b6b1ff37dde135e11108e31f204252")]
public partial class TryGetInputInteractState : Condition
{
    [SerializeReference] public BlackboardVariable<InteractState> InteractState;

    public override bool IsTrue()
    {
        bool result = PlayerInput.IsInputInteraction(out var state);
        if (result) InteractState.Value = state;
        return result;
    }
     



    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
