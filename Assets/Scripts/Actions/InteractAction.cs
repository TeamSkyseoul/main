using Battle;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractAction : ScriptableObject
{
    public abstract void Execute(IActor actor, IActor targetActor);
}

