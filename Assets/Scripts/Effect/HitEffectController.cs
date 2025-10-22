using Battle;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Effect
{
    public class HitEffectController
    {
        string address = "HitEffect";
        MaterialEffectMatrix materialEffect;

        public HitEffectController()
        {
            _ = InitializeAsync(address);
        }

        async Task InitializeAsync(string address)
        {
            AsyncOperationHandle<MaterialEffectMatrix> handle = Addressables.LoadAssetAsync< MaterialEffectMatrix>(address);
            await handle.Task;
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                materialEffect = handle.Result;
                materialEffect.BuildCache();
              
            }
            else
            {
                Debug.LogError($"[HitEffectController] Failed to load MaterialEffectTable from address: {address}");
            }
            Debug.Log($"[HitEffectController] :{materialEffect.rows.Count}");
            
        }
        
        MaterialEffectData GetEffect(string attacker, string victim)
        {
            return materialEffect?.Get(attacker, victim);
        }

        public void ShowHitEffect(HitBoxCollision collision)
        {
            if (!collision.Victim.Actor.TryGetComponent<ObjectSurfaceComponent>(out var victim) ||
                !collision.Attacker.Actor.TryGetComponent<ObjectSurfaceComponent>(out var attacker)) return;
        
           
            var data = GetEffect(attacker.MaterialType, victim.MaterialType);
            if (data == null)
            {
                Debug.Log($"[HitEffectController] : {attacker.MaterialType},{victim.MaterialType}간의 data가 없음 ");
            }


            Vector3 hitPoint = collision.HitPoint;

 
            Vector3 victimToAttacker = (collision.Attacker.Actor.transform.position - collision.Victim.Actor.transform.position).normalized;

     
            Vector3 normal = -victimToAttacker;


            Quaternion hitRot = Quaternion.LookRotation(normal);



            if (!string.IsNullOrEmpty(data.ParticleAddress)) 
                CreateEffectInstance(data.ParticleAddress, data.ParticleDuration, hitPoint, hitRot);


            if (!string.IsNullOrEmpty(data.DecalAddress)) 
                CreateEffectInstance(data.DecalAddress, data.DecalDuration, hitPoint, hitRot);

            if (data.ExtraParams.Count > 0)
            {
                foreach (var param in data.ExtraParams)
                {
                    ActionExtraEvent(param.Key, param.Value);
                }
            }
        }
        void DebugData(MaterialEffectData data)
        {
            Debug.Log(data.ParticleAddress);
            Debug.Log(data.DecalAddress);
            Debug.Log(data.ExtraParams);
        }

        async void CreateEffectInstance(string address, float duration, Vector3 position, Quaternion rotation)
        {
            var handle = Addressables.InstantiateAsync(address, position, rotation);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var obj = handle.Result;
                if (duration > 0) SetDestroyDuration(obj, duration);
            }
            else
            {
                Debug.LogError($"[HitEffectController] Failed to load prefab: {address}");
            }
        }


        public void ActionExtraEvent(string eventName, float duration)
        {
            //TODO EVENT LOGIC  
            Debug.Log($"[HitEffectController] Extra event triggered: {eventName}, duration: {duration}");
        }

        void SetDestroyDuration(GameObject obj, float duration) { GameObject.Destroy(obj, duration); }
    }
}
