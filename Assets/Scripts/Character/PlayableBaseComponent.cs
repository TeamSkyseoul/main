using Battle;
using UnityEngine;
using UnityEngine.ResourceManagement;

namespace Character
{
    public abstract class PlayableBaseComponent : CharacterBaseComponent, ISliding, IJumpable, IControlable, IPlayable
    {
        readonly IMove jump = new Jump();
        readonly IMove sliding = new Sliding();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            jump.SetActor(this);
            sliding.SetActor(this);
        }
        protected override void OnDie()
        {
            base.OnDie();
            (jump as IStrength)?.SetStrength(0f);
            (sliding as IStrength)?.SetStrength(0f);
        }
        void IJumpable.Jump()
        {
            animator.SetBool("IsGrounded", false);
            (jump as IStrength)?.SetStrength(3f);
            OnJump();
        }
        protected override void OnTakeDamage()
        {
            base.OnTakeDamage();
            (jump as IStrength)?.SetStrength(0f);
            (sliding as IStrength)?.SetStrength(0f);
        }
        protected virtual void OnJump() { }
        void ISliding.Slide()
        {
            animator.SetTrigger("Slide");
            (sliding as IDirection)?.SetDirection(transform.forward);
            (sliding as IStrength)?.SetStrength(15f);
            OnSlide();
        }
        protected virtual void OnSlide() { }
        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            (jump as IUpdateReceiver)?.Update(Time.fixedDeltaTime);
            (sliding as IUpdateReceiver)?.Update(Time.fixedDeltaTime);

            float jumpStrength = (jump as IStrength)?.GetStrength() ?? 0f;
            if (jumpStrength == 0) animator.SetBool("IsGrounded", IsGrounded);
        }

    }
}