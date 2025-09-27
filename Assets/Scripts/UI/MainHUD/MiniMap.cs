using UnityEngine;

namespace GameUI
{
    public class MiniMap :UIWidget
    {
        [Header("방향 공전")]
        [SerializeField] RectTransform rotateTransform;

        [Header("공전 반대방향 자전 객체들")]
        [SerializeField] RectTransform[] directionLabels;
       
        Transform target;

        private void Update()
        {
            if (target != null)
                RotateWithTarget();
        }
        void RotateWithTarget()
        {
            float playerY = target.eulerAngles.y;
            float currentZ = rotateTransform.localEulerAngles.z;
            float targetZ = -playerY;

            
            float smoothZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 5f);
            rotateTransform.localEulerAngles = new Vector3(0, 0, smoothZ);

            RotateLabels(playerY);
        }

        void RotateLabels(float playerY)
        {

            for (int i = 0; i < directionLabels.Length; i++)
            {
                float currentZ = directionLabels[i].localEulerAngles.z;
                float targetZ = playerY;

                
                float smoothZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 5f);
                directionLabels[i].localEulerAngles = new Vector3(0, 0, smoothZ);
            }
        }
        public void SetTarget(Transform target) { this.target = target; }
    }
}