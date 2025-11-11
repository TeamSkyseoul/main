using Battle;
using System;
using UnityEngine;

namespace Character
{
    public interface IStatusable { }
    public interface IGroundCheckable { bool IsGrounded { get; } }
    public interface IJumpable { void Jump(); }
    public interface IAttackable { void Attack(int attackType); }
    public interface IDamageable { void TakeDamage(); HitBox HitBox { get; } }
    public interface IDeathable { void Revive(); void Die(); bool IsDead { get; } float DeathDuration { get; set; } }
    public interface IMovable { void Move(Vector3 direction, float strength); }
    public interface ISliding { void Slide(); }
    public interface IHP { Statistics HP { get; } }
    public interface IControlable { }
    public interface ISkillOwner
    {
        public SkillComponent Skill { get; set; }
        public Vector3 SkillOffset { get; set; }
        public Vector3 SkillRotation { get; set; }
    }
    public interface ITraveler
    {
        void StartTravel();
        void EndTravel();
    }
    public interface IHitStun
    {
        void HitStun(float hitDuration);
        bool IsHit { get; }
    }
    public interface IStun
    {
        void Stun(float stunDuration);
        bool IsStun { get; }
    }
    public interface ICaster
    {
        void Invoke();
        void Cancel();
    }
    public interface IGrab
    {
        void Grab(Transform target);
        void Drop();
    }
    public interface IThrow
    {
        void Throw(Vector3 dir, float power);
    }
    public interface IAlert
    {
        void Alert();
        void Release();
    }

    public interface IRetriever 
    {
        float Duration { get; }
        Vector3 Offset { get;  }
        Vector3 Rotation { get; }
        void Retrieve(Transform actor);
    }
    public interface IExplode
    {
        void Explosion();
    }
    public interface IHackable { IActor Owner { get; } void Wake(IActor actor); bool IsWake { get; } float WakeDuration { get;} }

    public interface IInteractable
    {
        IActor CurrentActor { get; }
        float Progress { get; }
        bool CanBegin(IActor actor);
        void Begin(IActor actor); void Tick(IActor actor, float deltaTime); void Cancel();

        event Action<float> OnProgress;
        event Action OnCompleted, OnBegin, OnCancel;
    }
    public interface IInteractor
    {
        void BeginInteract(IActor actor);
        void Tick(IActor actor);
        void Cancel();
    }

}