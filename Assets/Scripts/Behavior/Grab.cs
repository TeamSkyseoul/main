using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Battle;
using Character;
using System.Linq;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Grab", story: "[Actor] grabs in [Distance]", category: "Action/Character", id: "f2b4b93c61c8d8ce773cf722cb9fa0a7")]
public partial class Grab : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<float> Distance;
    IActor actor;
    IGameObject target;
    NavMeshAgent agent;
    Vector3 startCharacterPos;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent(out actor)) { return Status.Failure; }

        //var entities = GameObject.FindObjectsByType<EntityBaseComponent>(FindObjectsSortMode.None).ToList();
        //var actores = entities.Where(x => x.gameObject.transform != Actor.Value)
        //    .Where(x => !x.gameObject.isStatic && x is IActor && x is IGameObject)
        //    .Where(x => Vector3.Distance(Actor.Value.transform.position, x.transform.position) < Distance.Value);
        //var orderby = actores.OrderBy(x => Vector3.Distance(Actor.Value.transform.position, x.transform.position))
        //    .OrderBy(x => x.gameObject.tag != "Grabbable");
        //target = orderby.Select(x => x as IGameObject).FirstOrDefault();

        target = GameObject.FindObjectsByType<EntityBaseComponent>(FindObjectsSortMode.None).ToList().Where(x => x.CompareTag("Grabbable")).FirstOrDefault();

        if (target == null) return Status.Failure;
        if (target is not IGameObject gameObject) return Status.Failure;
        if (!Actor.Value.transform.TryGetComponent(out agent)) return Status.Failure;
        if (IsAlreadyGrabed())
        {
            return Status.Success;
        }
        startCharacterPos = agent.transform.position;

        return Status.Running;
    }

    private bool IsAlreadyGrabed()
    {
        return agent.TryGetComponent<GrabInfo>(out var grabInfo) && grabInfo.GrabTransform.GetComponentsInChildren<Transform>().Any(x => x.Equals(target.transform)) && grabInfo.GrabTransform.parent != null;
    }

    public Vector3 GetPositionLeftOfBall(Vector3 character, Vector3 ball, float offsetDistance)
    {
        Vector3 front = (ball - character).normalized; front.y = 0;
        Vector3 left = Vector3.Cross(Vector3.up, front).normalized;
        return ball - left * offsetDistance;
    }

    protected override Status OnUpdate()
    {
        if (target.transform == null || !target.transform.gameObject.activeSelf) return Status.Failure;

        //if (agent.TryGetComponent<GrabInfo>(out var grabInfo))
        //{
        //    if (actor is ITraveler traveler) traveler.StartTravel();
        //    var grabPos = agent.transform.position - grabInfo.GrabTransform.position;
        //    grabPos.x = Mathf.Abs(grabPos.x);
        //    grabPos.z = Mathf.Abs(grabPos.z);
        //    grabPos.y = 0;

        //    var destination = GetPositionLeftOfBall(startCharacterPos, target.transform.position, grabPos.magnitude);
        //    agent.SetDestination(destination);
        //}

        //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        //{
        agent.isStopped = true;
        if (actor is ITraveler traveler) traveler.EndTravel();
        if (actor is not IGrab grabbable) return Status.Failure;
        grabbable.Grab(target.transform);
        return Status.Success;
        //}

        return Status.Running;
    }

    protected override void OnEnd()
    {
        agent.isStopped = false;
    }
}

