using Battle;


namespace Character
{
    public class RobotHackingTarget : InteractComponent
    {
        IHackable robot;

        protected override void Awake()
        {
            base.Awake();
            robot = GetComponent<IHackable>();
        }

        protected override void CompleteInteraction(IActor actor)
        {
            base.CompleteInteraction(actor);
            if (robot != null && !robot.IsWake) robot.Wake(actor);
        }

        public override bool CanBegin(IActor actor) =>  base.CanBegin(actor) && robot != null && !robot.IsWake;
    }
}