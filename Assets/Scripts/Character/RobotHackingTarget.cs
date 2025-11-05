using Battle;


namespace Character
{
    public class RobotHackingTarget : InteractComponent
    {
        IWakeable robot;

        protected override void Awake()
        {
            base.Awake();
            robot = GetComponent<IWakeable>();
        }

        protected override void CompleteInteraction(IActor actor)
        {
            base.CompleteInteraction(actor);
            if (robot != null && !robot.IsWake) robot.Wake();
        }

        public override bool CanBegin(IActor actor) =>  base.CanBegin(actor) && robot != null && !robot.IsWake;
    }
}