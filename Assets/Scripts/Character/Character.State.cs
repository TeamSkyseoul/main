using Battle;
using UnityEngine;

namespace Chaeracter.State
{
    public static class ActorState
    {
        const float footSize = 0.3f;
        static RaycastHit[] hits;

        public static bool IsGrounded(IActor actor)
        {
            if (actor is ITransform gameObject)
            {
                var ray = new Ray(gameObject.transform.position + Vector3.up * 0.5f, Vector3.down);
                var hitCount = Physics.SphereCastNonAlloc(ray, footSize, hits, footSize, LayerMask.GetMask("Ground"));
                var IsGrounded = hitCount != 0;
                return IsGrounded;
            }

            return true;
        }
    }
}