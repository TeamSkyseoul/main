using UnityEngine.ResourceManagement;
using UnityEngine;
using UnityEngine.AI;
using Battle;

namespace Character
{
    public class Walk : IMove, IDirection, IStrength, IUpdateReceiver
    {
        float strength;
        Transform actorTransform;
        Vector3 direction;
        NavMeshAgent agent;

        void IMove.SetActor(IActor actor)
        {
            if (actor is not ITransform gameObject || !gameObject.transform.TryGetComponent<NavMeshAgent>(out var agent))
            {
                return;
            }

            this.actorTransform = gameObject.transform;
            this.agent = agent;
        }
        void IDirection.SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }
        void IStrength.SetStrength(float strength)
        {
            this.strength = strength;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            strength *= 1f - Mathf.Clamp01(1.5f * unscaledDeltaTime);
            strength = strength < 0.1f ? 0f : strength;
            if (actorTransform == null || agent == null || !agent.enabled || !agent.isOnNavMesh) return;
            var dir = direction;
            SetDirOfForward(ref dir);
            agent.Move(dir * strength * unscaledDeltaTime);
        }
        void SetDirOfForward(ref Vector3 dir)
        {
            if (actorTransform == null) return;

            var @base = actorTransform.transform;
            dir = @base.forward * dir.z + @base.right * dir.x;
            dir.y = 0;
            dir = dir.normalized;
        }
    }
    public class Jump : IMove, IStrength, IUpdateReceiver
    {
        float strength;
        Transform actorTransform;
        NavMeshAgent agent;
        IGroundCheckable groundCheckable;

        void IMove.SetActor(IActor actor)
        {
            if (actor is not ITransform gameObject || !gameObject.transform.TryGetComponent<NavMeshAgent>(out var agent))
            {
                Debug.LogWarning($"Actor is not NavMeshAgent");
                return;
            }

            if (actor is not IGroundCheckable groundCheckable)
            {
                Debug.LogWarning($"Actor is not {nameof(IGroundCheckable)}");
                return;
            }

            this.actorTransform = gameObject.transform;
            this.agent = agent;
            this.groundCheckable = groundCheckable;
        }
        public void SetStrength(float strength)
        {
            if (actorTransform is null)
            {
                Debug.LogWarning($"Actor not found.");
                return;
            }

            this.strength = strength;
            agent.updatePosition = false;
            agent.transform.position += Vector3.up * strength;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actorTransform is null || strength == 0) return;
            agent.updatePosition = groundCheckable.IsGrounded;
            strength = groundCheckable.IsGrounded ? 0 : strength;
        }
    }
    public class Sliding : IMove, IDirection, IStrength, IUpdateReceiver
    {
        float strength;
        float startStrength;
        float startTime;
        Vector3 direction;
        IActor actor;
        NavMeshAgent agent;

        void IMove.SetActor(IActor actor)
        {
            if (actor is not ITransform gameObejct || !gameObejct.transform.TryGetComponent<NavMeshAgent>(out var agent))
            {
                return;
            }
            this.actor = actor;
            this.agent = agent;
        }
        void IDirection.SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }
        void IStrength.SetStrength(float strength)
        {
            this.startStrength = strength;
            this.strength = strength;
            this.startTime = Time.time;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actor == null || strength == 0) return;
            var t = Mathf.Clamp01(Time.time - startTime);
            strength = startStrength * (1 - t);
            agent.Move(strength * unscaledDeltaTime * direction);
        }
    }
    public class ReciveGravity : IMove, IUpdateReceiver
    {
        Transform actorTransform;
        IGroundCheckable groundCheckable;

        void IMove.SetActor(IActor actor)
        {
            if (actor is not ITransform gameObject)
            {
                return;
            }
            if (actor is not IGroundCheckable groundCheckable)
            {
                return;
            }
            this.actorTransform = gameObject.transform;
            this.groundCheckable = groundCheckable;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actorTransform == null) return;
            if (!groundCheckable.IsGrounded) actorTransform.transform.position += Physics.gravity * unscaledDeltaTime /2f;
        }
    }
}