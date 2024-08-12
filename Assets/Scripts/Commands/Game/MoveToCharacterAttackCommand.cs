﻿using Controllers.Game;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class MoveToCharacterAttackCommand : BaseCommand
    {
        private string _characterName;
        private Rigidbody2D _rg;

        private CharacterStats _characterStats;

        public MoveToCharacterAttackCommand(string characterName)
        {
            _characterName = characterName;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            if (!GamePlayModel.Characters.ContainsKey(_characterName))
            {
                return;
            }

            _characterStats = GamePlayModel.Characters[_characterName];
            _rg = _characterStats.Transform.GetComponent<Rigidbody2D>();

            MoveToCharacterAttack();
        }

        private void MoveToCharacterAttack()
        {
            if (_characterStats.IsAttackCharacter || _characterStats.IsAttackTarget)
            {
                return;
            }

            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPointX = _characterStats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f;
                var newPoint = new Vector3(newPointX, characterNearestPos.y);

                var velocity = (newPoint - _characterStats.Transform.position).normalized * CharacterConfig.speed;
                _rg.velocity = velocity;
            }
            else
            {
                _rg.velocity = _characterStats.Target.normalized * CharacterConfig.speed;
            }
        }

        private Character GetCharacterAttackNearest()
        {
            Character characterNearest = null;
            float minDistance = 10;
            foreach (var (_, characterStats) in GamePlayModel.CharactersAttacking)
            {
                if (_characterStats.Type == characterStats.Type || _characterStats.IsDeath || characterStats.IsDeath)
                {
                    continue;
                }

                var distance = Vector3.Distance(_characterStats.Transform.position, characterStats.Transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    characterNearest = characterStats.Transform.GetComponent<Character>();
                }
            }

            return characterNearest;
        }
    }
}