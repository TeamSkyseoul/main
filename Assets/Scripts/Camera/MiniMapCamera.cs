using Battle;
using GameUI;
using Unity.VisualScripting;
using UnityEngine;

namespace GameCamera
{
    public class MiniMapCamera : MonoBehaviour
    {
        [Header("위치 오프셋")]
        [SerializeField] Vector3 offset;

        Transform target;
        
        public void Init(IActor target)
        {
            Component comp = target as Component;
            if (comp == null)
            {
                Debug.LogError($"[MiniMapCamera] {target} 는 Component가 아님.");
                return;
            }

            this.target = comp.transform;

            GameUI.BattleHUD hud = (GameUI.BattleHUD)UIController.Instance.MainHUD;
            hud.SettingMiniMapTarget(transform);
        }
        void LateUpdate()
        {
            if (target == null) return;

            
            transform.position = target.position + offset;

            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }



    }
}
