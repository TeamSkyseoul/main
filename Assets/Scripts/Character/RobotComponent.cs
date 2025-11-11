using UnityEngine;
using Battle;
using System.Collections.Generic;

namespace Character
{
    public class RobotComponent : PropBaseComponent, IMovable, IAttackable, IHP, IHackable
    {
        [Header("Base Stats")]
        [SerializeField] Statistics hp = new(10);

        [Header("Skill Actions")]
        [SerializeField] List<SkillAction> skills = new();

        readonly Dictionary<SkillTriggerType, List<SkillAction>> skillTable = new();
        readonly IMove walk = new Walk();

        public Statistics HP => hp;
        public IActor Owner { get; private set; }
        public bool IsWake { get; private set; }
        public float WakeDuration { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            BuildSkillTable();
        }

        void BuildSkillTable()
        {
            skillTable.Clear();
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                if (skill == null) continue;

                skill.Initialize();

                if (!skillTable.ContainsKey(skill.TriggerType))
                    skillTable.Add(skill.TriggerType, new List<SkillAction>());

                skillTable[skill.TriggerType].Add(skill);
            }
        }

  
        #region Movement
        void IMovable.Move(Vector3 direction, float strength)
        {
            if (direction.sqrMagnitude < 0.01f) return;

            direction.Normalize();
            animator?.SetBool("IsMove", true);

            if (walk is IStrength s) s.SetStrength(strength);
            if (walk is IDirection d) d.SetDirection(direction);

            OnMove(direction, strength);
        }

        protected virtual void OnMove(Vector3 direction, float strength) { }

        public virtual void StopMove() => animator?.SetBool("IsMove", false);
        #endregion
     

        public virtual void Attack(int attackType = 0)
        {
            animator?.SetTrigger("Attack");
            animator?.SetInteger("AttackType", attackType);

            TryExecuteSkills(SkillTriggerType.OnAttack);
        }

        protected override void OnTakeDamage()
        {
            TryExecuteSkills(SkillTriggerType.OnHit);

            if (hp.Value <= 0 && !IsDead)
                (this as IDeathable)?.Die();
        }

        public virtual void Wake(IActor actor)
        {
            Debug.Log($"[RobotComponent]: {actor}");


            Owner = actor;
            IsWake = true;
            TryExecuteSkills(SkillTriggerType.OnWake);
        }

        protected override void OnDie()
        {
            base.OnDie();
            TryExecuteSkills(SkillTriggerType.OnDeath);
        }

        #region Skill Execution
        void TryExecuteSkills(SkillTriggerType type)
        {
            if (!skillTable.TryGetValue(type, out var skillList)) return;

           
            for (int i = 0; i < skillList.Count; i++)
            {
                var skill = skillList[i];
                if (skill == null) continue;
                skill.Execute(transform);
            }
        }
        #endregion
    }
}
