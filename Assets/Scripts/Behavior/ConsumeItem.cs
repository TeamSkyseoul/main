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
       
        //TODO: UICONTROLLER -> USE ITEM 
        if (UIController.Instance.MainHUD is BattleHUD hud)
        {
            //TODO : 아이템 획득 기능 전까지 3번 4번으로 NOTIFY CHECK
            if (SlotIndex.Value== 3)
            {
                hud.ShowMessage("ItemAcquired", "권총");
            }
            else if(SlotIndex.Value==4)
            {
                hud.ShowMessage("EnemyAppeared", "좀비");
            }
            else
            {
                hud.ConsumeItem(SlotIndex.Value);
            }
            return Status.Success;
        }

        Debug.LogWarning("[ConsumeItem] BattleHUD가 로드되지 않음");
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

