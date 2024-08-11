using System.Collections;
using Controllers.Game;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCharacterCommand : BaseCommand
    {
        private CharacterStats _statsBeaten;
        private CharacterStats _statsAttack;

        private string _keyBeaten;
        private string _keyAttack;

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

            // _statsBeaten.Health.Value -= _statsAttack.Damage;
        }
        
         public void AttackCharacter(Character characterBeaten)
        {
            if (!characterBeaten)
            {
                _statsAttack.IsAttack = false;
                return;
            }
            
            _keyBeaten = characterBeaten.name;
            
            GamePlayModel.CharactersAttacking.TryAdd(_statsAttack.Name, _statsAttack);
            _statsAttack.CharactersCanBeaten.TryAdd(_keyBeaten, characterBeaten);
            
            if (_statsAttack.IsAttack)
            {
                return;
            }
            
            _statsAttack.IsAttack = true;
            
            // _attackCharacterIE = AttackCharacterIE();
            // StartCoroutine(_attackCharacterIE);
        }
        
        private IEnumerator AttackCharacterIE()
        {
            yield return new WaitForSeconds(CharacterConfig.attackTime);
            var isCharacterBeaten = GamePlayModel.Characters.ContainsKey(_keyBeaten);
        
            if (!isCharacterBeaten)
            {
                _statsAttack.CharactersCanBeaten.Remove(_keyBeaten);
                _keyBeaten = GetCharacterCanBeaten();
            }
            else
            {
                // this.SendCommand(new AttackCommand(keyBeaten, name));
                var statsBeaten = GamePlayModel.Characters[_keyBeaten];
                statsBeaten.Health.Value -= _statsAttack.Damage;
            }
        
            if (_keyBeaten is null)
            {
                _statsAttack.IsAttack = false;
                // StopCoroutine(_attackCharacterIE);
                // MoveToCharacterAttack();
                yield break;
            }
        
            // _attackCharacterIE = AttackCharacterIE();
            //
            // StartCoroutine(_attackCharacterIE);
        }
        
        private string GetCharacterCanBeaten()
        {
            string keyBeaten = null;
            foreach (var (_, character) in _statsAttack.CharactersCanBeaten)
            {
                if (character.Stats.IsDeath)
                {
                    continue;
                }
        
                keyBeaten = character.name;
                break;
            }
        
            return keyBeaten;
        }

    }
}