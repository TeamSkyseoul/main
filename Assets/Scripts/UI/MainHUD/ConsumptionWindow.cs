using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class ConsumptionWindow : UIWidget
    {
        [Header("Consumption List")]
        [SerializeField] List<MonoBehaviour> slotComponents; 
        List<ISetValue> slots;

        private void Awake()
        {
            SlotsCasting();
        }

        void SlotsCasting()
        {
            slots = new List<ISetValue>();
            for (int i = 0; i < slotComponents.Count; i++)
            {
                if (slotComponents[i] is ISetValue<int> setValue)
                {
                    slots.Add(setValue);
                }
                else
                {
                    Debug.LogWarning($"[ConsumptionWindow] {slotComponents[i]} 은 ISetValue<int>를 구현하지 않음");
                }
            }
        }

     
        private T GetSlot<T>(int index) where T : class, ISetValue<int>
        {
            int slotIndex = index - 1;
            if (slotIndex < 0 || slotIndex >= slots.Count)
            {
                Debug.LogWarning($"[ConsumptionWindow] 잘못된 index 요청: {index}");
                return null;
            }

            return slots[slotIndex] as T;
        }

     
        public void Consume(int index, int amount = 1)
        {
            var consumptionSlot = GetSlot<Consumption>(index);
            if (consumptionSlot != null)
            {
                consumptionSlot.Consume(amount);
            }
        }

        
        public void SetValue(int index, int value)
        {
            var slot = GetSlot<ISetValue<int>>(index);
            slot?.SetValue(value);
        }
    }
}
