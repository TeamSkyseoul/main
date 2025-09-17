using Battle;
using Character;
using System;
using TMPro;
using UnityEngine;

namespace GameUI
{   
    public class ObjectStatus :WorldUI
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] StatusBar hpStatusBar;
        [SerializeField] StatusBar poiseStatusBar;

        Transform target;
        Vector3 offset = new Vector3(0.0f, 0.8f, 0.0f);

     
        private void Update()
        {
            if (target != null)
            {
                Debug.Log(target.gameObject.name);
                transform.position = target.position + offset;
            }
        }
            

            
        public void SetName(string name) => nameText.text = name;
        public void UpdateHp(float hp) => hpStatusBar.UpdateStatusBar(hp);
        public void UpdatePoise(float poise)=> poiseStatusBar.UpdateStatusBar(poise);

        public void Bind(Transform  target, IHP ihp)
        {
            this.target = target;
            UpdateHp(ihp.HP.Ratio);
        }
    
        public void Release()
        {
            Unbind();
            InvokeRelease();
        }
        public void Unbind()
        {
            target = null;
        }
           

    }
}