using Controllers.Game;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCommand : BaseCommand
    {
        private Character _characterBeaten;
        private Character _characterAttack;

        public AttackCommand(Character characterBeaten, Character characterAttack)
        {
            _characterBeaten = characterBeaten;
            _characterAttack = characterAttack;
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            await AttackAsync();
        }

        private async UniTask AttackAsync()
        {
            var characterBeatenId = _characterBeaten.Stats.ID;
            var characterAttackId = _characterAttack.Stats.ID;
            await UniTask.WaitForSeconds(ActorConfig.attackTime);

            if (_characterAttack.Stats.IsDeath || !_characterBeaten ||
                _characterBeaten.Stats.IsDeath || _characterBeaten.Stats.ID != characterBeatenId
                || _characterAttack.Stats.ID != characterAttackId)
            {
                return;
            }

            if (_characterBeaten.Stats.IsDeath)
            {
                foreach (var (_, character) in _characterAttack.Stats.CharactersCanBeaten)
                {
                    if (!character.Stats.IsDeath)
                    {
                        _characterBeaten = character;
                        await AttackAsync();
                        break;
                    }
                }

                return;
            }

            _characterBeaten.Stats.Health.Value -= _characterAttack.Stats.Damage;

            await AttackAsync();
        }
    }
}