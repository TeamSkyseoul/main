using UnityEngine;
using System;
using INab.Common;

namespace Effect
{
    public interface IAppearance
    {
        float Duration { get; }
        void InvokeDissolve();
        void InvokeAppear();
    }
   
    public class VFXAppearanceComponent : MonoBehaviour, IAppearance
    {
        
        [Header("Duration")]
        [SerializeField,Range(0,10)] float duration;

        [Header("VFX")]
        [SerializeField] InteractiveEffect dissolve;
        [SerializeField] InteractiveEffect appear;

        [SerializeField] bool withLifeCycle;
        public float Duration => duration;


        void OnEnable()
        {
            if (withLifeCycle)
                InvokeAppear();
        }

        void OnDisable()
        {
            if  (withLifeCycle)
                InvokeDissolve();
        }

        void AppearEffect() => appear?.PlayEffect();
        void DissolveEffect()=> dissolve?.PlayEffect();
        public void InvokeDissolve() => DissolveEffect();
        public void InvokeAppear() => AppearEffect();
       

#if UNITY_EDITOR
        public void SyncDuration()
        {
            if (dissolve != null) dissolve.duration = duration;
            if (appear != null) appear.duration = duration;
        }
#endif
    }
}
