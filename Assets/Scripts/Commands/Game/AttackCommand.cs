using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCommand : BaseCommand
    {
        private CharacterStats _statsBeaten;
        private CharacterStats _statsAttack;

        private string _keyBeaten;
        private string _keyAttack;

        public AttackCommand(string keyBeaten, string keyAttack)
        {
            _keyBeaten = keyBeaten;
            _keyAttack = keyAttack;
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            await AttackAsync();
        }

        private async UniTask AttackAsync()
        {
            await UniTask.WaitForSeconds(CharacterConfig.attackTime);

            if (!GamePlayModel.Characters.ContainsKey(_keyBeaten)
                || !GamePlayModel.Characters.ContainsKey(_keyAttack))
            {
                return;
            }

            _statsBeaten = GamePlayModel.Characters[_keyBeaten];
            _statsAttack = GamePlayModel.Characters[_keyAttack];

            if (_statsAttack.IsDeath || _statsBeaten.IsDeath)
            {
                return;
            }

            if (_statsBeaten.IsDeath)
            {
                foreach (var (_, character) in _statsAttack.CharactersCanBeaten)
                {
                    if (!character.Stats.IsDeath)
                    {
                        _statsBeaten = character.Stats;
                        await AttackAsync();
                        break;
                    }
                }

                return;
            }

            _statsBeaten.Health.Value -= _statsAttack.Damage;

            await AttackAsync();
        }
    }
}