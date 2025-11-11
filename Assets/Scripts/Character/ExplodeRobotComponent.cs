using UnityEngine;
namespace Character
{
    public class ExplodeRobotComponent : RobotComponent, IExplode
    {
        [SerializeField] SkillComponent explodeSkill;
      
        public void Explosion()
        {
            explodeSkill.SetCaster(transform);
            explodeSkill.Fire();
        }
    }
}