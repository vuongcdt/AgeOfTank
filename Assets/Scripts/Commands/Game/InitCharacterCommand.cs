﻿using Interfaces;
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

        protected override  void OnExecute()
        {
            base.OnExecute();
            InitCharacter();
        }

        private async void InitCharacter()
        {
            ActorConfig = await  this.GetSystem<ConfigSystem>().GetCharacterConfig();
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

            var health = ActorConfig.unitConfigs[(int)_typeClass].health;
            var damage = ActorConfig.unitConfigs[(int)_typeClass].damage;
            var tag = isPlayer ? CONSTANS.Tag.Player : CONSTANS.Tag.Enemy;
            var name = $"{tag} {id}";
            var source = isPlayer ? ActorConfig.pointSource : ActorConfig.pointTarget;
            var target = !isPlayer ? ActorConfig.pointSource : ActorConfig.pointTarget;

            GamePlayModel.Characters.Add(name,
                new CharacterStats(health, id, damage, target, source, tag, name, _typeClass));
            GamePlayModel.InitCharacterKey.Value = name;
        }
    }
}