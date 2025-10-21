using Character;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class BattleHUD : UIHUD
    {
  
        public void UpdatePlayerHp(IHP health) => GetWidget<PlayerStatus>().UpdatePlayerHp(health.HP.Ratio);
        public void UpdatePlayerStamina(float ratio) => GetWidget<PlayerStatus>().UpdateImpairment(ratio);
        public void ShowMessage(string type, string target) => GetWidget<Notification>().ShowMessage(type, target);
        public void ConsumeItem(int slotIndex, int amount = 1 ) => GetWidget<ConsumptionWindow>().Consume(slotIndex,amount);
        public void SettingMiniMapTarget(Transform target)=>GetWidget<MiniMap>().SetTarget(target);

    }

} 

