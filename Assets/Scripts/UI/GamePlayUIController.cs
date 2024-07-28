using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class GamePlayUIController : BaseGamePlayUiController
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text sliderText;
        [SerializeField] private CardCharacterUIController[] cards;

        private void Start()
        {
            SetTypeCard();

            slider.value = 0;
            sliderText.text = "0";

            GamePlayModel.ProductFoodProgress.RegisterWithInitValue(progress => { slider.value = progress; });

            GamePlayModel.FoodNum.RegisterWithInitValue(UpdateSliderText);
        }

        private async void SetTypeCard()
        {
            await UniTask.WaitForEndOfFrame(this);

            for (var index = 0; index < cards.Length; index++)
            {
                cards[index].InitCard((ENUMS.CharacterTypeClass)index);
            }
        }

        private void UpdateSliderText(int newValue)
        {
            sliderText.text = newValue.ToString();
        }
    }
}