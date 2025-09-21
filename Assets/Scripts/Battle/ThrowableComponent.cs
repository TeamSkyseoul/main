using Unity.AppUI.Core;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class ThrowableComponent : WeaponBaseComponent
{
    [SerializeField] AttackBoxComponent container;
    [SerializeField] float threshold;
    Rigidbody rigid;
    bool throwed;
    Transform parent;
    Vector3 localPosition;
    Quaternion localRotation;

    public void Throw(Vector3 dir, float power)
    {
        if (!container || !rigid) return;
        OnThrowStart(dir, power);
    }

    protected override void Initialize(Transform owner)
    {
        if (container == null) return;
        container.SetActor(owner);
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        parent = transform.parent;
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
    }
    void Update()
    {
        Util.Enumerator.InvokeFor(transform.GetComponentsInChildren<BehaviorGraphAgent>(), x => x.enabled = false);

        if (!throwed) return;
        if (container.AttackBox.NotWithinAttackWindow) container.OpenAttackWindow();
        throwed = threshold < rigid.linearVelocity.magnitude;
        if (!throwed) OnThrowEnd();
    }
    void Reset()
    {
        rigid.linearVelocity = Vector3.zero;
        rigid.isKinematic = true;
        transform.SetParent(parent);
        transform.SetLocalPositionAndRotation(localPosition, localRotation);
    }

    void OnThrowStart(Vector3 dir, float power)
    {
        Reset();
        throwed = true;
        transform.SetParent(null);
        rigid.isKinematic = false;
        rigid.linearVelocity = dir;
        rigid.AddForce(dir * power, ForceMode.Impulse);
    }
    void OnThrowEnd()
    {
        Util.Enumerator.InvokeFor(transform.GetComponentsInChildren<BehaviorGraphAgent>(), x => x.enabled = true);
        transform.DetachChildren();
        Reset();
    }
}