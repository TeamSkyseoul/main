using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Battle
{
    public class HitEffectController 
    {
        MaterialEffectTable materialEffect;

        //TODO : 추후 많이 쓰이면 따로 빼는 게 좋을듯 
        public async Task InitializeAsync(string address)
        {
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var json = handle.Result.text;
                materialEffect = ScriptableObject.CreateInstance<MaterialEffectTable>();
                JsonUtility.FromJsonOverwrite(json, materialEffect);

                materialEffect.BuildCache();
            }
            else
            {
                Debug.LogError($"[HitEffectController] Failed to load MaterialEffectTable from address: {address}");
            }
        }

    }
}
