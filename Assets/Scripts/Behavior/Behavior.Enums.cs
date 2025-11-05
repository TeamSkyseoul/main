using Unity.Behavior;

[BlackboardEnum]
public enum BattleStatus
{
    Idle,
    Fleeing,
    Attacking,
    Patrolling,
    Tracing
}

[BlackboardEnum]
public enum AbnormalStatus
{
    HitStun,
    Stun,
    Die
}

[BlackboardEnum]
public enum BasicStatus
{
    IsGrounded,
    IsAiring
}
[BlackboardEnum]
public enum RobotSubStatus
{
    Sleep=0,
    Wake=1<<0
}