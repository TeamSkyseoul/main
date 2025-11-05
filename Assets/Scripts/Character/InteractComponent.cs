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
    public event Action OnCompleted;
    public event Action OnBegin;
    public event Action OnCancel;

    public float Progress => state.Progress;

    protected virtual void Awake()
    {
        if (anchor == null)
            anchor = transform;
    }

    protected virtual void Update()
    {
        state.TickDecay(Time.deltaTime, profile.DecaySpeed, OnProgress);
    }

    public virtual bool CanBegin(IActor actor)
    {
       
        if (actor is not Component comp) return false;
        return Vector3.Distance(anchor.position, comp.transform.position) <= profile.Range;
    }


    public virtual void Begin(IActor actor)
    {
        if (!CanBegin(actor)) return;

        currentActor = actor;
        state.Begin();
        OnBegin?.Invoke();
    }

   
    public virtual void Tick(IActor actor, float deltaTime)
    {
        if (!state.IsActive) return;

        state.Tick(deltaTime, OnProgress);
        Debug.Log($"Tick... HeldTime={state.HeldTime:F2}, Required={profile.HoldDuration:F2}");

        if (state.IsCompleted(profile.HoldDuration))
        {
            Debug.Log(" Interaction Complete");
            CompleteInteraction(actor);
        }
    }
    public virtual void Cancel(IActor actor)
    {
        if (!state.IsActive) return;
        state.Cancel();
        OnCancel?.Invoke();
    }

    protected virtual void CompleteInteraction(IActor actor)
    {
        OnCompleted?.Invoke();

        for(int i=0; i<actions.Count;i++)
            actions[i]?.Execute(transform);
      
        state.Reset(OnProgress);
        currentActor = null;
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(anchor ? anchor.position : transform.position, profile.Range);
    }
#endif
}
