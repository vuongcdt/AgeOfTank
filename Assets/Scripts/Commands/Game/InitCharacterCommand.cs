using Interfaces;
using QFramework;
using Systems;
using Utilities;

namespace Commands.Game
{
    public class InitCharacterCommand : BaseCommand
    {
        private CharacterConfig _characterConfig;
        private ENUMS.CharacterTypeClass _typeClass;

        public InitCharacterCommand(ENUMS.CharacterTypeClass typeClass)
        {
            _typeClass = typeClass;
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
            InitCharacter();
        }

        private void InitCharacter()
        {
            var isPlayer = (int)_typeClass < 3;
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

            var health = _characterConfig.unitConfigs[(int)_typeClass].health;
            var damage = _characterConfig.unitConfigs[(int)_typeClass].damage;
            var tag = isPlayer ? CONSTANS.Tag.Player : CONSTANS.Tag.Enemy;
            var name = $"{tag} {id}";
            var source = isPlayer ? _characterConfig.pointSource : _characterConfig.pointTarget;
            var target = !isPlayer ? _characterConfig.pointSource : _characterConfig.pointTarget;

            GamePlayModel.Characters.Add(name,
                new CharacterStats(health, id, damage, target, source, tag, name, _typeClass));
            GamePlayModel.InitCharacterKey.Value = name;
        }
    }
}