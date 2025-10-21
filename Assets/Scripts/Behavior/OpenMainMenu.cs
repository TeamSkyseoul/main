using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameUI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OpenMainMenu", story: "Open MainMenu Or  ClosePopUp", category: "Action", id: "1af6c34dc1b2187d6daf80badee2614e")]
public partial class OpenMainMenu : Action
{
    
    protected override Status OnStart()
    {
        UIController ui = UIController.Instance;
      
        if (ui.IsAnyPopUp()) ui.CloseTopPopUp();
        else ui.ShowPopup<MainMenu>();
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

