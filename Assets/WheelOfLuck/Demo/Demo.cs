using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfLuck.Scripts;

namespace WheelOfLuck.Demo
{
   public class Demo : MonoBehaviour 
   {
      private const string FREE_TEXT = "Free";
      private const string GOLD_PRICE_TEXT = "1 Gold";
      [SerializeField] 
      private Button _spinButton ;
      [SerializeField]
      private TextMeshProUGUI _textButton;
      [SerializeField]
      private int _goldCount;
      [SerializeField]
      private int _spinPrice;   
      [SerializeField]
      private TextMeshProUGUI _textGoldCount;
      [SerializeField]
      private PickerWheel _pickerWheel;

      private bool _isUseFreeSpin = true;
   
      private void Awake()
      {
         _spinButton.onClick.AddListener(Spin);
         _pickerWheel.OnSpinStart += UnActiveButton;
         _pickerWheel.OnSpinEnd += ActiveButton;
      }

      private void Start()
      {
         UpdateButtonText();
         _textGoldCount.text = $"Gold {_goldCount}";
      }

      private void OnDestroy()
      {
         _spinButton.onClick.RemoveListener(Spin);
         _pickerWheel.OnSpinStart -= UnActiveButton;
         _pickerWheel.OnSpinEnd -= ActiveButton;
      }

      private void ActiveButton (int obj)
      {
         _spinButton.interactable = true;
         UpdateButtonText();
      }

      private void UnActiveButton()
      {
         _textGoldCount.text = $"Gold {_goldCount}";
         
         _spinButton.interactable = false;
      }

      private void Spin()
      {
         if (_isUseFreeSpin)
         {
            _textButton.text = FREE_TEXT;
            _pickerWheel.Spin();
            _isUseFreeSpin = false;
            return;
         }
         if(_goldCount<_spinPrice)
         {
            _spinButton.interactable = false;
            return;
         }
         _textButton.text = GOLD_PRICE_TEXT;
         _goldCount -= _spinPrice;
         _pickerWheel.Spin();
      }

      private void UpdateButtonText()
      {
         _textButton.text = _isUseFreeSpin ? FREE_TEXT : GOLD_PRICE_TEXT;
      }
   }
}
