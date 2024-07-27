using Commands.GamePlayUI;
using Controllers.Game;
using QFramework;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class CardCharacterUIController : BaseGameController
    {
        [SerializeField] private Image avatar;
        [SerializeField] private TMP_Text foodNumText;
        [SerializeField] private Image bgCard;

        private ENUMS.CharacterTypeClass _typeClass;
        private int _foodNum;
        private Button _button;
        private readonly Color _colorDisable = new(0.12f, 0.13f, 0.22f, 0.96f);
        private readonly Color _colorActive = new(0.08f, 0.39f, 0.9f, 1);

        private void Start()
        {
            _button = GetComponent<Button>();
            GamePlayModel.FoodNum.RegisterWithInitValue(CheckFoodNum);
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

        private void OnClickCard()
        {
            this.SendCommand(new ConsumeFoodNumCommand(_foodNum));
            this.SendEvent(new Events.Events.InitCharacter(_typeClass));
        }

        public async void InitCard(ENUMS.CharacterTypeClass typeClass)
        {
            var characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();

            _typeClass = typeClass;
            foodNumText.color = Color.red;
            bgCard.color = _colorDisable;

            avatar.sprite = characterConfig.unitConfigs[(int)typeClass].imgAvatar;
            _foodNum = characterConfig.unitConfigs[(int)typeClass].foodNum;
            foodNumText.text = _foodNum.ToString();
        }
    }
}