using Character;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TryGetInputBool", story: "TryGet [input] out [value]", category: "Input", id: "f6a47ce7ba84d5ea97aa363d363a1200")]
public partial class TryGetInputBool : Condition
{
    [SerializeReference] public BlackboardVariable<PcInputBool> Input;
    [SerializeReference] public BlackboardVariable<bool> Value;

    public override bool IsTrue()
    {
        switch (Input.Value)
        {
            case PcInputBool.Jump:
                return PlayerInput.IsInputJump();
            case PcInputBool.Sliding:
                return PlayerInput.IsInputSlide();
            default:
                return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
