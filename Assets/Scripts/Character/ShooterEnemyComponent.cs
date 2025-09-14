using Character;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ShooterEnemyComponent : EnemyComponent
{
    [SerializeField] ThrowableComponent throwable;
    protected override void OnThrow(Vector3 dir, float power)
    {
        base.OnThrow(dir, power);
        if (throwable == null) return;
        StartCoroutine(LateThrow(dir, power));
    }
    IEnumerator LateThrow(Vector3 dir, float power)
    {
        var delay = 0f;
        if (TryGetComponent<ThrowInfo>(out var throwInfo)) { delay = throwInfo.ThrowDelay; }
        yield return new WaitForSeconds(delay);
        throwable.transform.SetParent(null);
        throwable.Throw(dir, power);
        Util.Enumerator.InvokeFor(throwable.GetComponentsInChildren<NavMeshObstacle>(), x => x.enabled = true);
    }
    protected override void OnGrab(Transform target)
    {
        base.OnGrab(target);
        Util.Enumerator.InvokeFor(throwable.GetComponentsInChildren<NavMeshObstacle>(), x => x.enabled = false);
    }
}