using Commands.GamePlayUICommands;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.GamePlayUI
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

            slider.DOValue(1, 1 / GamePlayUIModel.FoodPerSecond.Value)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .OnStepComplete(this.SendCommand<AddFoodCommand>);

            GamePlayUIModel.FoodNum.RegisterWithInitValue(UpdateSliderText);
        }

        private void SetTypeCard()
        {
            for (var index = 0; index < cards.Length; index++)
            {
                cards[index].Type = (CONSTANTS.CardCharacterType)index;
            }
        }

        private void UpdateSliderText(int newValue)
        {
            sliderText.text = newValue.ToString();
        }
    }
}