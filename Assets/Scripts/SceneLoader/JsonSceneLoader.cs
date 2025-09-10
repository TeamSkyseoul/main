using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using TopDown;
using GameUI;


namespace SceneLoad
{
    public class JsonSceneLoader : TopDown.Loader
    {
        CancellationTokenSource _cancellationTokenSource;
        readonly List<AsyncOperationHandle> _handles = new();
        readonly List<GameObject> _instantiatedObjects = new();
        readonly Dictionary<int, GameObject> _instantiatedObjectsById = new();
        readonly Dictionary<string, GameObject> _prefabCache = new();

        public JsonSceneLoader(string name)
        {
            SettingLocator(name);
         #if UNITY_EDITOR
            AttachEditorSafeHelper();
         #endif
        }

       

        public void SettingLocator(string sceneName)
        {
            Locator = sceneName;
        }

        public override void Load()
        {
            _ = LoadAsync();
        }



        async Task LoadAsync()
        {
            await Addressables.InitializeAsync().Task;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            string key = Locator;
            string json = await ReadJsonAsync(key);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"[JsonSceneLoader] JSON 로드 실패: {key}");
                InvokeOnFailure();
                return;
            }

            List<SceneObjectData> dataList = DeserializeJson(json);
            if (dataList == null  ||dataList.Count == 0)
            {
                Debug.LogError("[JsonSceneLoader] JSON 파싱 실패 또는 오브젝트 없음.");
                InvokeOnFailure();
                return;
            }

            await PreloadPrefabs(dataList, token);

            for (int i = 0; i < dataList.Count; i++)
            {
                if (token.IsCancellationRequested) return;

                var data = dataList[i];
                Transform parent = GetParent(data.parentID);
                GameObject obj = await InstantiatePrefab(data, parent, token);

                if (obj == null)
                {
                    Debug.LogError($"[JsonSceneLoader] 인스턴스 생성 실패: ID={data.ID}");
                    InvokeOnFailure();
                    return;
                }

                InvokeOnProgress((float)(i + 1) / dataList.Count);
                if (i % 5 == 0) await Task.Yield();
            }
            InvokeOnSuccess();
        }

        Transform GetParent(int id)
        {
            return _instantiatedObjectsById.TryGetValue(id, out var parentObject) ? parentObject.transform : null;
        }

        async Task<string> ReadJsonAsync(string key)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(key);
            _handles.Add(handle);
            await handle.Task;
            return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result.text : null;
        }

        List<SceneObjectData> DeserializeJson(string json)
        {
            try
            {
                var wrapper = JsonUtility.FromJson<SerializationWrapper<SceneObjectData>>(json);
                return wrapper?.Items ?? new List<SceneObjectData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonSceneLoader] JSON 파싱 오류: {ex.Message}");
                return new List<SceneObjectData>();
            }
        }

        async Task PreloadPrefabs(List<SceneObjectData> dataList, CancellationToken token)
        {
            var addresses = dataList
                .Where(d => !string.IsNullOrEmpty(d.PrefabAddress))
                .Select(d => d.PrefabAddress)
                .Distinct()
                .Where(addr => !_prefabCache.ContainsKey(addr))
                .ToList();

            List<Task> preloadTasks = new();

            foreach (var address in addresses)
            {
                if (token.IsCancellationRequested) return;

                var handle = Addressables.LoadAssetAsync<GameObject>(address);
                _handles.Add(handle);

                var task = handle.Task.ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully && handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _prefabCache[address] = handle.Result;
                    }
                    else
                    {
                        Debug.LogError($"[JsonSceneLoader] 프리팹 프리로드 실패: {address}");
                    }
                }, token);

                preloadTasks.Add(task);
            }

            await Task.WhenAll(preloadTasks);
        }

        async Task<GameObject> InstantiatePrefab(SceneObjectData data, Transform parent, CancellationToken token)
        {
            if (token.IsCancellationRequested) return null;

            GameObject instance = null;

            if (!string.IsNullOrEmpty(data.PrefabAddress))
            {
                instance = await InstantiatePrefabObject(data, token);
                if (instance == null)
                {
                    Debug.LogError($"[JsonSceneLoader] 프리팹 인스턴스화 실패: {data.PrefabAddress}");
                    return null;
                }

                if (parent != null)
                {
                    instance.transform.SetParent(parent, false);
                    instance.transform.SetSiblingIndex(data.SiblingIndex);
                }
                ApplyTransform(instance.transform, data);
            }
            else if (data.SiblingIndex >= 0 && parent != null)
            {
                instance = FindContainer(data, parent);
                if (instance == null)
                {
                    Debug.LogError($"[JsonSceneLoader] 부모 ID {data.parentID}에서 컨테이너 객체를 찾을 수 없습니다.");
                    return null;
                }
            }

            if (instance != null)
            {
                _instantiatedObjectsById[data.ID] = instance;
            }

            return instance;
        }

        async Task<GameObject> InstantiatePrefabObject(SceneObjectData data, CancellationToken token)
        {
            if (_prefabCache.TryGetValue(data.PrefabAddress, out var prefab))
            {
                var instance = GameObject.Instantiate(prefab);
                _instantiatedObjects.Add(instance);
                return instance;
            }
            else
            {
                var handle = Addressables.InstantiateAsync(data.PrefabAddress);
                _handles.Add(handle);
                await handle.Task;

                if (token.IsCancellationRequested || handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"[JsonSceneLoader] 인스턴스화 실패: {data.PrefabAddress}");
                    return null;
                }

                var instance = handle.Result;
                _instantiatedObjects.Add(instance);
                return instance;
            }
        }

        GameObject FindContainer(SceneObjectData data, Transform parent)
        {
            if (_instantiatedObjectsById.TryGetValue(data.parentID, out var parentObj) && data.SiblingIndex >= 0)
            {
                Transform container = parentObj.transform.GetChild(data.SiblingIndex);
                ApplyTransform(container, data);
                return container.gameObject;
            }

            Debug.LogWarning($"[JsonSceneLoader] 유효하지 않은 containerIndex: {data.SiblingIndex}, 부모 ID: {data.parentID}");
            return null;
        }

        void ApplyTransform(Transform transform, SceneObjectData data)
        {
            transform.localPosition = data.Position;
            transform.localRotation = data.Rotation;
            transform.localScale = data.Scale;
        }

        #region Unload
        public override void Unload()
        {
            CancelLoading();
            ReleaseInstantiatedObjects();
            ReleaseLoadedAssets();

            _instantiatedObjectsById?.Clear();
            _prefabCache?.Clear();
        }

        void CancelLoading()
        {
            if (_cancellationTokenSource != null)
            {
                try
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                        _cancellationTokenSource.Cancel();
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    try { _cancellationTokenSource.Dispose(); }
                    catch { }
                    _cancellationTokenSource = null;
                }
            }
        }

        public void ReleaseInstantiatedObjects()
        {
            for (int i = _instantiatedObjects.Count - 1; i >= 0; i--)
            {
                var obj = _instantiatedObjects[i];
                if (obj == null) continue;

                try
                {
                    if (Addressables.ResourceManager != null)
                        Addressables.ReleaseInstance(obj);
                    else
                        GameObject.Destroy(obj);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[JsonSceneLoader] ReleaseInstance 실패: {e.Message}");
                }
            }
            _instantiatedObjects.Clear();
        }

        void ReleaseLoadedAssets()
        {
            for (int i = _handles.Count - 1; i >= 0; i--)
            {
                var handle = _handles[i];
                try
                {
                    if (handle.IsValid())
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                            Addressables.Release(handle);
                        else
                            Addressables.Release(handle); 
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[JsonSceneLoader] Release 핸들 예외: {ex.Message}");
                }
            }
            _handles.Clear();
        }

        #endregion

        #region  UnityEditor
        void AttachEditorSafeHelper()
        {
            var existing = GameObject.Find(nameof(JsonSceneLoaderComponent));
            if (existing != null) return;

           
            var safeGo = new GameObject(nameof(JsonSceneLoaderComponent));
            UnityEngine.Object.DontDestroyOnLoad(safeGo);

            var safeComp = safeGo.AddComponent<GameUI.JsonSceneLoaderComponent>();
            safeComp.SetLoader(this);
        }
        #endregion
    }

}