using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Resource
{
    public interface IResourceManager
    {
        Task<T> LoadAsync<T>(string key) where T : UnityEngine.Object;
        void Release<T>(T obj) where T : UnityEngine.Object;
    }

    public class AddressableResourceManager : MonoBehaviour, IResourceManager
    {
        public static AddressableResourceManager Instance { get; private set; }

        private readonly Dictionary<string, AsyncOperationHandle> cache = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task<T> LoadAsync<T>(string key) where T : UnityEngine.Object
        {
            if (cache.TryGetValue(key, out var handle))
                return (T)handle.Result;

            var newHandle = Addressables.LoadAssetAsync<T>(key);
            cache[key] = newHandle;
            return await newHandle.Task;
        }

        public void Release<T>(T obj) where T : UnityEngine.Object
        {
            var key = obj.name;
            if (cache.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                cache.Remove(key);
            }
        }
    }

}