using Character;
using Effect;
using System.Collections;
using UnityEngine;

namespace Character
{
    public class RetrieverComponent : PropBaseComponent, IRetriever
    {
        public float Duration => duration;
        public Vector3 Offset => retrieve.Offset;
        public Vector3 Rotation => retrieve.Rotation;

        [SerializeField] bool durationWithEffect;
        [SerializeField] float duration;
        [SerializeField] RetrieveOffset retrieve;

        public void Retrieve(Transform actor)
        {
            if (actor == null) return;
            StartCoroutine(RetrieveRoutine(actor));
        }
        void DissolveAndRetrieve(Transform actor)
        {
            actor.gameObject.SetActive(false);
            ApplyRetrieveTransform(actor);
        }

        IEnumerator RetrieveRoutine(Transform actor)
        {
            actor.TryGetComponent(out IAppearance appearance);
            appearance?.InvokeDissolve();         

            yield return new WaitForSeconds(CalDisappearTime(appearance));

            DissolveAndRetrieve(actor);

            animator.SetTrigger("Working");

            actor.gameObject.SetActive(true);

            appearance?.InvokeAppear();
        }

        float CalDisappearTime(IAppearance appearance) =>
            (appearance != null && durationWithEffect) ? appearance.Duration : Duration;

       
        void ApplyRetrieveTransform(Transform actor)
        {
            actor.SetPositionAndRotation(
                transform.position + Offset,
                transform.rotation * Quaternion.Euler(Rotation)
            );
        }
    }

    [System.Serializable]
    public struct RetrieveOffset
    {
        public Vector3 Offset;
        public Vector3 Rotation;
    }
}
