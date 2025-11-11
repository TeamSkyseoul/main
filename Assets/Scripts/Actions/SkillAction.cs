using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum SkillTriggerType
{
    None,
    OnAttack,
    OnDeath,
    OnHit,
    OnWake,
    OnSpawn,
}


public abstract class SkillAction : ScriptableObject, IInitializable
{
    [Header("Settings")]
    public SkillTriggerType TriggerType = SkillTriggerType.None;

    public abstract void Execute(Transform owner);

    public virtual void Initialize() { }
}
