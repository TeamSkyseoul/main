using Character;
using GameUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


namespace Battle
{
    public class BattleController : IController
    {
        public event Action<IActor> OnDead;
        GameUI.BattleHUD battleHUD;
        //readonly BattleHUD battleHUD = new();
        readonly HashSet<IActor> joinCharacters = new();

        public BattleController() { battleHUD = UIController.Instance.ShowHUD<GameUI.BattleHUD>(); }
        public void Update()
        {

        }
        public void Clear()
        {
            while (joinCharacters.Count > 0)
            {
                DisposeCharacter(joinCharacters.First());
            }
            battleHUD.Hide();
        }
        public void JoinCharacter(IActor actor)
        {
            if (actor is IDamageable body)
                body.HitBox.OnCollision += OnHitCharacter;
            joinCharacters.Add(actor);
        }
        public void DisposeCharacter(IActor actor)
        {
            if (actor is IDamageable body)
                body.HitBox.OnCollision -= OnHitCharacter;
            joinCharacters.Remove(actor);
        }
        void OnHitCharacter(HitBoxCollision collision)
        {
            if (!collision.Victim.Actor.TryGetComponent<IActor>(out var actor)) return;
            if (actor is IDeathable death && death.IsDead) return;
            if (actor is IHP health)
            {
                Action<IHP> updateHUD = actor switch
                {
                    IPlayable => battleHUD.UpdatePlayerHp,
                    IEnemy => hp => UIController.WorldUI.UpdateStatus(actor, hp),
                    _ => hp => UIController.WorldUI.UpdateStatus(actor, hp)
                };
                updateHUD.Invoke(health);
                health.HP.Value--;
                updateHUD.Invoke(health);
                if (health.HP.Value <= 0) DoDie(actor);
                else if (actor is IDamageable damageable) damageable.TakeDamage();
            }


            else if (actor is IDamageable damageable) damageable.TakeDamage();
        }
        void DoDie(IActor actor)
        {
            this.DisposeCharacter(actor);
            if (actor is IDeathable death) death.Die();
            OnDead?.Invoke(actor);
        }
    }
}
