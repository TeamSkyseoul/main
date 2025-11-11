using System;
using System.Collections.Generic;
using UnityEngine;
using Battle;
using Character;


[DisallowMultipleComponent]
public class InteractComponent : MonoBehaviour, IInteractable
{
 
    [SerializeField] InteractionProfile profile = new();
   
    [SerializeField]  List<InteractAction> actions = new();

    [SerializeField] Transform anchor;

    InteractionState state = new();
    IActor currentActor;

    public event Action<float> OnProgress;
    public event Action OnCompleted, OnBegin,OnCancel;
   

    public IActor CurrentActor => currentActor;
    public float Progress => state.Progress;

    protected virtual void Awake()
    {
        if (anchor == null) anchor = transform;

    }

    protected virtual void Update()
    {
        state.TickDecay(Time.deltaTime, profile.DecaySpeed, OnProgress);
    }

    public virtual bool CanBegin(IActor actor)
    {
        if (actor is not IGameObject obj || obj.transform == null)
            return false;
        return Vector3.Distance(anchor.position, obj.transform.position) <= profile.InteractRange;
    }


    public virtual void Begin(IActor actor)
    {
        if (!CanBegin(actor)) return;

        OnBegin?.Invoke();
        currentActor = actor;
        state.Begin();
    }

   
    public virtual void Tick(IActor actor, float deltaTime)
    {
        if (!state.IsActive)
        {
            Begin(actor);
            return;
        }

        state.Tick(deltaTime, OnProgress);

        if (state.IsCompleted(profile.HoldDuration)) CompleteInteraction(actor);
            
    }
    public virtual void Cancel()
    {
        if (!state.IsActive) return;
        currentActor = null;
        state.Cancel();
        OnCancel?.Invoke();
    }

    protected virtual void CompleteInteraction(IActor actor)
    {
        OnCompleted?.Invoke();

        if(TryGetComponent<IActor>(out var target))
        for(int i=0; i<actions.Count;i++) actions[i]?.Execute(actor,target);

        state.Reset(OnProgress);
        currentActor = null;
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(anchor ? anchor.position : transform.position, profile.InteractRange);
    }
#endif
}
