using UnityEngine;
using UnityEngine.UI;

namespace WheelOfLuck.Scripts
{
    public class PieceView : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Text _label;
        [SerializeField]
        private Text _amount;

      
        public void  SetPieceView (Sprite icon, string label, string amount)
        {
            _icon.sprite = icon;
            _label.text = label;
            _amount.text = amount;
        }
    }
}