using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ValiableBetween", story: "[A] is [true] Between [B] and [C]", category: "Variable Conditions", id: "486ccbc39a64a3d4c079411f95903f96")]
public partial class ValiableBetween : Condition
{
    [SerializeReference] public BlackboardVariable<float> A;
    [SerializeReference] public BlackboardVariable<float> B;
    [SerializeReference] public BlackboardVariable<float> C;
    [SerializeReference] public BlackboardVariable<bool> True;

    public override bool IsTrue()
    {
        var min = Mathf.Min(B.Value, C.Value);
        var max = Mathf.Min(B.Value, C.Value);

        return True.Value == (min <= A.Value && A.Value <= max);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
