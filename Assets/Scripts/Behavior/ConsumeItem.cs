using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameUI;
using UnityEngine.UIElements;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ConsumeItem", story: "Consume [slotIndex] Item", category: "Action", id: "4d0f000be0a81d116443b3f15b2a9a81")]
public partial class ConsumeItem : Action
{
    [SerializeReference] public BlackboardVariable<int> SlotIndex;

    protected override Status OnStart()
    {
     
        if (UIController.Instance.MainHUD is BattleHUD hud)
        {
            
            if (SlotIndex.Value== 3) hud.ShowMessage("ItemAcquired", "±ÇÃÑ");
            else if(SlotIndex.Value==4) hud.ShowMessage("EnemyAppeared", "Á»ºñ");
            else hud.ConsumeItem(SlotIndex.Value);
            return Status.Success;
        }

        return Status.Failure;

    }


      

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

