using Battle;
using Character;
using System;
using TMPro;
using UnityEngine;

namespace GameUI
{   
    public class ObjectStatus : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] StatusBar hpStatusBar;
        [SerializeField] StatusBar poiseStatusBar;

        Transform target;
        Vector3 offset = Vector3.zero;

        private void Update()
        {
            if (target != null)
                transform.position = target.position + offset;
        }
            

            
        public void SetName(string name) => nameText.text = name;
        public void UpdateHp(float hp) => hpStatusBar.UpdateStatusBar(hp);
        public void UpdatePoise(float poise)=> poiseStatusBar.UpdateStatusBar(poise);

        public void Bind(Transform  target, IHP ihp)
        {
            this.target = target;
            UpdateHp(ihp.HP.Ratio);
        }
    

        public void Unbind()
        {
            target = null;
        }
           

    }
}