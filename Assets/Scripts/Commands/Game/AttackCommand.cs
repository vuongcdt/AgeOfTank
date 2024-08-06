using Controllers.Game;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            await UniTask.WaitForSeconds(ActorConfig.attackTime);
            
            if (_characterAttack.Stats.IsDeath || !_characterBeaten)
            {
                return;
            }
            
            if (_characterBeaten.Stats.IsDeath)
            {
                foreach (var pair in _characterAttack.Stats.CharactersCanBeaten)
                {
                    if (!pair.Value.Stats.IsDeath)
                    {
                        _characterBeaten = pair.Value;
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