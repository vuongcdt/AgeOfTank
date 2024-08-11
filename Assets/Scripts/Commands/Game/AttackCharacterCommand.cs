using System.Threading;
using Cysharp.Threading.Tasks;
using Interfaces;
using QFramework;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCharacterCommand : BaseCommand
    {
        private CharacterStats _statsBeaten;
        private CharacterStats _statsAttack;

        private string _keyBeaten;
        private string _keyAttack;

        private CancellationTokenSource _cancelAttackCharacter = new();

        public AttackCharacterCommand(string keyBeaten, string keyAttack)
        {
            _keyBeaten = keyBeaten;
            _keyAttack = keyAttack;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            if (!GamePlayModel.Characters.ContainsKey(_keyBeaten)
                || !GamePlayModel.Characters.ContainsKey(_keyAttack))
            {
                return;
            }

            _statsBeaten = GamePlayModel.Characters[_keyBeaten];
            _statsAttack = GamePlayModel.Characters[_keyAttack];
            
            AttackCharacter();
        }

        private void AttackCharacter()
        {
            _keyBeaten = _statsBeaten.Name;

            GamePlayModel.CharactersAttacking.TryAdd(_statsAttack.Name, _statsAttack);
            _statsAttack.CharactersCanBeaten.TryAdd(_keyBeaten, _statsBeaten);

            if (_statsAttack.IsAttack)
            {
                return;
            }

            _statsAttack.IsAttack = true;

            AttackCharacterAsync();
        }

        private async void AttackCharacterAsync()
        {
            await UniTask.WaitForSeconds(CharacterConfig.attackTime,
                cancellationToken: _cancelAttackCharacter.Token);

            var isCharacterBeaten = GamePlayModel.Characters.ContainsKey(_keyBeaten);
            var isCharacterAttack= GamePlayModel.Characters.ContainsKey(_keyAttack);
            if (!isCharacterAttack)
            {
                return;
            }

            if (!isCharacterBeaten)
            {
                _statsAttack.CharactersCanBeaten.Remove(_keyBeaten);
                _keyBeaten = GetCharacterCanBeaten();
            }
            else
            {
                var statsBeaten = GamePlayModel.Characters[_keyBeaten];
                statsBeaten.Health.Value -= _statsAttack.Damage;
            }

            if (_keyBeaten is null)
            {
                _statsAttack.IsAttack = false;
                if (!_cancelAttackCharacter.IsCancellationRequested)
                {
                    _cancelAttackCharacter.Cancel();
                }

                // MoveToCharacterAttack();
                this.SendCommand(new MoveToCharacterAttackCommand(_statsAttack.Name));
                return;
            }

            AttackCharacterAsync();
        }

        private string GetCharacterCanBeaten()
        {
            string keyBeaten = null;
            foreach (var (_, characterStats) in _statsAttack.CharactersCanBeaten)
            {
                if (_statsAttack.IsDeath)
                {
                    continue;
                }

                keyBeaten = characterStats.Name;
                break;
            }

            return keyBeaten;
        }
    }
}