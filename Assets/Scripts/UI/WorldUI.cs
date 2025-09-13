using Battle;
using Character;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class WorldUI
    {
        readonly Dictionary<IActor, ObjectStatus> statusBars = new();
        readonly Queue<ObjectStatus> statusPool = new();
        Transform worldCanvasRoot;

        ~WorldUI() { Clear(); }
        string GetStatusPrefabPath(IActor actor)
        {
            return actor switch
            {
                IEnemy => "UI/EnemyStatus",
                _ => "UI/AllyStatus"
            };
        }

        public Transform WorldCanvasRoot
        {
            get
            {
                if (worldCanvasRoot == null)
                {
                    var go = new GameObject("@WorldCanvas");
                    var canvas = go.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = Camera.main;
                    worldCanvasRoot = go.transform;
                }
                return worldCanvasRoot;
            }
        }

        ObjectStatus CreateStatus(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            var go = Object.Instantiate(prefab, WorldCanvasRoot);
            var status = go.GetComponent<ObjectStatus>();
            go.SetActive(false);
            return status;
        }

        public ObjectStatus ShowStatus(IActor actor, IHP hp)
        {
            ObjectStatus status = statusPool.Count > 0 ? statusPool.Dequeue() : CreateStatus(GetStatusPrefabPath(actor));
            status.gameObject.SetActive(true);

            if (actor is Component comp)
            {
                Transform attach = comp.transform;
                var animator = comp.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    var head = animator.GetBoneTransform(HumanBodyBones.Head);
                    if (head != null) attach = head;
                }
                    
                status.Bind(attach, hp);
                status.OnReleased = s => HideStatus(actor);
            }

            statusBars[actor] = status;
            return status;
        }

        public void HideStatus(IActor actor)
        {
            if (statusBars.TryGetValue(actor, out var status))
            {
                statusBars.Remove(actor);
                status.Unbind();
                status.gameObject.SetActive(false);
                statusPool.Enqueue(status);
            }
        }

        public void Clear()
        {
            foreach (var kv in statusBars)
            {
                kv.Value.Unbind();
                kv.Value.gameObject.SetActive(false);
                statusPool.Enqueue(kv.Value);
            }
            statusBars.Clear();
        }

        public void UpdateStatus(IActor actor, IHP hp)
        {
            if (statusBars.TryGetValue(actor, out var status))
                status.UpdateHp(hp.HP.Ratio);
            else
                ShowStatus(actor, hp);
        }
    }
}
