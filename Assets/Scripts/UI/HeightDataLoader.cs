using GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.EventSystems.EventTrigger;

public class HeightDataLoader
{
    HeightDataBase heightDataBase;
    Dictionary<string, Vector3> heights = new();
    public HeightDataLoader()
    {
        Load(nameof(HeightDataBase));
    }
    public void Load(string addressKey)
    {
        Addressables.LoadAssetAsync<TextAsset>(addressKey).Completed += OnLoaded;
    }

    void OnLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var json = handle.Result.text;
            heightDataBase = JsonUtility.FromJson<HeightDataBase>(json);

            for (int i = 0; i < heightDataBase.entries.Count; i++)
            {
                heights[heightDataBase.entries[i].ID] = new Vector3(0, heightDataBase.entries[i].Height, 0);
            }
          
        }
    }


    public Vector3 GetHeight(string ID)
    {
        if (ID.EndsWith("(Clone)"))
            ID = ID.Replace("(Clone)", "");

        if (heights.TryGetValue(ID, out var height))
            return height;
   
        Debug.LogWarning($"[HeightDataLoader] ID not found: {ID}");
        return Vector3.zero;
    }

}
