using System.Collections;
using UnityEngine;
using Battle;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "AttackSkillAction", menuName = "Skills/AttackSkillAction")]
public class AttackSkillAction : SkillAction
{
    [Header("Prefab")]
    public SkillComponent skillPrefab;

    [Header("Offset Settings")]
    public Vector3 Position;
    public Vector3 Rotation;

    SkillComponent skillInstance;

    public override void Initialize()
    {

        if (skillPrefab != null)
        {
            skillInstance = Object.Instantiate(skillPrefab);
            skillInstance.gameObject.SetActive(false);
        }
    }

    public override void Execute(Transform owner)
    {
        if (skillInstance == null) return;

        Debug.Log("ATTACK");
        skillInstance.gameObject.SetActive(true);
        skillInstance.SetCaster(owner);

        skillInstance.transform.position = owner.position + Position;
        skillInstance.transform.eulerAngles = owner.eulerAngles + Rotation;

        skillInstance.Fire();

       
          
    }
}
