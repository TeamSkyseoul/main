using TMPro;
using UnityEngine;

namespace GameUI
{
    public class Consumption : MonoBehaviour,ISetValue<int>
    {
        [SerializeField]
        TextMeshProUGUI countText;
        int value = 3;
        public void SetValue(int amount) 
        { 
            value = amount;
            WriteCount();
        }
        public void Consume(int amount)
        { 
            value -= amount;
            WriteCount();
        }

        public void Add(int amount) { value += amount; }
        void WriteCount() {countText.text = value.ToString(); } 
    }
     
}