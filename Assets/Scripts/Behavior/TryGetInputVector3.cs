using Character;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TryGetInputVector3", story: "TryGet [input] out [value]", category: "Input", id: "ddb75117bdbc43fb1d0bcc26b03b20ca")]
public partial class TryGetInputVector3 : Condition
{
    [SerializeReference] public BlackboardVariable<PcInputVector3> Input;
    [SerializeReference] public BlackboardVariable<Vector3> Value;

    public override bool IsTrue()
    {
        switch (Input.Value)
        {
            case PcInputVector3.Run:
                {
                    var input = PlayerInput.IsInputRun(out var moveDir);
                    Value.Value = moveDir;
                    return input;
                }
            case PcInputVector3.Walk:
                {
                    var input = PlayerInput.IsInputWalk(out var moveDir);
                    Value.Value = moveDir;
                    return input;
                }
            default: return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
