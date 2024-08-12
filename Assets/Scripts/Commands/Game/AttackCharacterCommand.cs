﻿using Cysharp.Threading.Tasks;
using Interfaces;
using QFramework;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCharacterCommand : BaseCommand
    {
        private string _keyBeaten;
        private string _keyAttack;

        private Rigidbody2D _rg;
        private CharacterStats _statsBeaten;
        private CharacterStats _statsAttack;

        public AttackCharacterCommand(string keyBeaten, string keyAttack)
        {
            _keyBeaten = keyBeaten;
            _keyAttack = keyAttack;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            AttackCharacter();
        }

        private void AttackCharacter()
        {
            if (!GamePlayModel.Characters.ContainsKey(_keyBeaten)
                || !GamePlayModel.Characters.ContainsKey(_keyAttack))
            {
                return;
            }

            _statsAttack = GamePlayModel.Characters[_keyAttack];
            _statsBeaten = GamePlayModel.Characters[_keyBeaten];

            _statsAttack.CharactersCanBeaten.TryAdd(_keyBeaten, _statsBeaten);
                
            _rg = _statsAttack.Transform.GetComponent<Rigidbody2D>();
            _rg.mass = CharacterConfig.mass;
            _rg.velocity = Vector3.zero;
            
            if (_statsAttack.IsAttackCharacter)
            {
                return;
            }

            _statsAttack.IsAttackCharacter = true;
            GamePlayModel.CharactersAttacking.TryAdd(_keyAttack, _statsAttack);

            AttackCharacterAsync();
        }

        private async void AttackCharacterAsync()
        {
            await UniTask.WaitForSeconds(CharacterConfig.attackTime);

            var isCharacterBeaten = GamePlayModel.Characters.ContainsKey(_keyBeaten);
            var isCharacterAttack = GamePlayModel.CharactersAttacking.ContainsKey(_keyAttack);

            if (!isCharacterAttack)
            {
                return;
            }

            if (isCharacterBeaten)
            {
                var statsBeaten = GamePlayModel.Characters[_keyBeaten];
                statsBeaten.Health.Value -= _statsAttack.Damage;
                AttackCharacterAsync();
                return;
            }

            _statsAttack.CharactersCanBeaten.Remove(_keyBeaten);
            _keyBeaten = GetCharacterCanBeaten();

            if (_keyBeaten != null)
            {
                AttackCharacterAsync();
                return;
            }

            _statsAttack.IsAttackCharacter = false;

            this.SendCommand(new MoveToCharacterAttackCommand(_statsAttack.Name));
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