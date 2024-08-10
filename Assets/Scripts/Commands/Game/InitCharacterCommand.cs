using Interfaces;
using QFramework;
using Systems;
using Utilities;

namespace Commands.Game
{
    public class InitCharacterCommand : BaseCommand
    {
        private ENUMS.CharacterTypeClass _typeClass;

        public InitCharacterCommand(ENUMS.CharacterTypeClass typeClass)
        {
            _typeClass = typeClass;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            InitCharacter();
        }

        private async void InitCharacter()
        {
            CharacterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
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

            var health = CharacterConfig.unitConfigs[(int)_typeClass].health;
            var damage = CharacterConfig.unitConfigs[(int)_typeClass].damage;
            var tag = isPlayer ? CONSTANS.Tag.Player : CONSTANS.Tag.Enemy;
            var name = $"{tag} {id}";
            var source = isPlayer ? CharacterConfig.pointSource : CharacterConfig.pointTarget;
            var target = !isPlayer ? CharacterConfig.pointSource : CharacterConfig.pointTarget;
            var type = isPlayer ? ENUMS.CharacterType.Player : ENUMS.CharacterType.Enemy;

            var characterStats = new CharacterStats(health, id, damage, target, source, tag, name, _typeClass, type);
            GamePlayModel.Characters.Add(name, characterStats);
            GamePlayModel.InitCharacterKey.Value = name;
        }
    }
}