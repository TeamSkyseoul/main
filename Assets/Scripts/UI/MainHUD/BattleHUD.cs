using Character;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class BattleHUD : UIHUD
    {
        PlayerStatus statusBar;
        ConsumptionWindow consumption;
        Notification notification;
        public override bool Init()
        {
            if (!base.Init()) return false;
            GetBattleWidgets();
            return true;
        }

        void GetBattleWidgets()
        {
            statusBar = GetWidget<PlayerStatus>();
            notification = GetWidget<Notification>();    
            consumption = GetWidget<ConsumptionWindow>();       
        }

        public void UpdatePlayerHp(IHP health) => statusBar.UpdatePlayerHp(health.HP.Ratio);
        public void UpdatePlayerStamina(float ratio) => statusBar.UpdateImpairment(ratio);
        public void ShowMessage(string type, string target) => notification.ShowMessage(type, target);
        public void ConsumeItem(int slotIndex, int amount = 1 ) => consumption.Consume(slotIndex,amount);


    }

} 

