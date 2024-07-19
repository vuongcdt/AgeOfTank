using Commands.GamePlayUICommands;
using QFramework;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.GamePlayUI
{
    public class CardCharacterUIController : BaseGamePlayUiController
    {
        [SerializeField] private Image avatar;
        [SerializeField] private TMP_Text foodNumText;
        [SerializeField] private Image bgCard;
        [SerializeField] private GameUIData gameUIData;

        private CONSTANTS.CardCharacterType _type;
        private int _foodNum;
        private Button _button;
        private readonly Color _colorDisable = new(0.12f, 0.13f, 0.22f, 0.96f);
        private readonly Color _colorActive = new(0.08f, 0.39f, 0.9f, 1);

        public CONSTANTS.CardCharacterType Type
        {
            get => _type;
            set => SetData(value);
        }

        private void Start()
        {
            _button = GetComponent<Button>();
            GamePlayUIModel.FoodNum.RegisterWithInitValue(CheckFoodNum);
        }

        private void OnClickCard()
        {
            this.SendCommand(new ConsumeFoodNumCommand(_foodNum));
            this.SendEvent(new Events.Events.InitCharacter(_type));
        }

        private void CheckFoodNum(int newValue)
        {
            if (newValue < _foodNum)
            {
                foodNumText.color = Color.red;
                bgCard.color = _colorDisable;
                _button.onClick.RemoveAllListeners();
            }
            else
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnClickCard);

                foodNumText.color = Color.white;
                bgCard.color = _colorActive;
            }
        }

        private void SetData(CONSTANTS.CardCharacterType type)
        {
            _type = type;
            foodNumText.color = Color.red;
            bgCard.color = _colorDisable;

            avatar.sprite = gameUIData.imgAvatar[(int)type];
            _foodNum = gameUIData.foodNumCards[(int)type];
            foodNumText.text = _foodNum.ToString();
        }
    }
}