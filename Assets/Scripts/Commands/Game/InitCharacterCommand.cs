using Interfaces;
using QFramework;
using Systems;

namespace Commands.Game
{
    public class InitCharacterCommand : BaseCommand
    {
        private CharacterConfig _characterConfig;
        private CONSTANTS.CardCharacterType _type;

        public InitCharacterCommand(CONSTANTS.CardCharacterType type)
        {
            _type = type;
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
            InitCharacter();
        }

        private void InitCharacter()
        {
            var isPlayer = (int)_type < 3;
            int id;
            if (isPlayer)
            {
                GamePlayModel.IdPlayer.Value++;
                id = GamePlayModel.IdPlayer.Value;
            }
            else
            {
                GamePlayModel.IdEnemy.Value++;
                id = GamePlayModel.IdEnemy.Value;
            }

            var health = _characterConfig.unitConfigs[(int)_type].health;
            var damage = _characterConfig.unitConfigs[(int)_type].damage;
            var tag = isPlayer ? CONSTANTS.Tag.Player : CONSTANTS.Tag.Enemy;
            var name = $"{tag} {id}";
            var source = isPlayer ? _characterConfig.pointSource : _characterConfig.pointTarget;
            var target = !isPlayer ? _characterConfig.pointSource : _characterConfig.pointTarget;

            GamePlayModel.Characters.Add(name,
                new CharacterStats(health, id, damage, target, source, tag, name, _type));
            GamePlayModel.InitCharacterKey.Value = name;
        }
    }
}