using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
    public class HighlightButton : MonoBehaviour
    {
        [SerializeField] Image hightlightImage;
        [SerializeField] Sprite emptySprite;
        [SerializeField] Sprite highlightSprite;
        
        void SetImage(Sprite sprite) { hightlightImage.sprite = sprite; }
        public void SetHighlight(bool highlight) { SetImage(highlight ? highlightSprite : emptySprite); }
    }

}