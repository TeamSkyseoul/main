using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using UnityEngine.UIElements;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Interact", story: "[Self] Interacts by [interactState]", category: "Action", id: "954fe4992daea623f01793f50d849594")]
public partial class Interact : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<InteractState> interactState;

    protected override Status OnStart()
    {
        if (!Self.Value.TryGetComponent<IActor>(out var actor)||
            !Self.Value.TryGetComponent<IInteractor>(out var interactor))
            return Status.Failure;
        switch (interactState.Value)
        {
            case InteractState.Begin:
                interactor.BeginInteract(actor);
                return Status.Success;
            case InteractState.Tick:
                interactor.Tick(actor);
                return Status.Success;
            case InteractState.Cancel:
                interactor.Cancel();
                return Status.Success;

        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

