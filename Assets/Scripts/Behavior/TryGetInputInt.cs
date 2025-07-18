using Character;
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.WSA;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TryGetInputInt", story: "TryGet [input] out [value]", category: "Input", id: "541580397f43551ee9522019dbf2bb88")]
public partial class TryGetInputInt : Condition
{
    [SerializeReference] public BlackboardVariable<PcInputInt> Input;
    [SerializeReference] public BlackboardVariable<int> Value;

    public override bool IsTrue()
    {
        switch (Input.Value)
        {
            case PcInputInt.MeleeAttack:
                var input = PlayerInput.IsInputMeleeAttack(out var attackNum);
                Value.Value = attackNum;
                return input;
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
