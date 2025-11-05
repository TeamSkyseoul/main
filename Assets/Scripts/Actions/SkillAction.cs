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

    [Tooltip("다른 스킬과 병렬 실행이 가능한가?")]
    public bool CanRunParallel = true;

    public abstract void Execute(Transform owner);

    public virtual void Initialize() { }
}
