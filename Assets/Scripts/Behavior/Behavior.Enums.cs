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
    None,
    Hacking,
    FollowingPlayer,
    PreparingExplosion,
    Exploding,
    Cooldown
}