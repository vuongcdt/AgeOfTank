using Controllers.Game;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class MoveToCharacterAttackCommand : BaseCommand
    {
        private string _keyAttack;
        private Rigidbody2D _rg;

        private CharacterStats _stats;

        public MoveToCharacterAttackCommand(string keyAttack)
        {
            _keyAttack = keyAttack;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            if (!GamePlayModel.CharactersAttacking.ContainsKey(_keyAttack))
            {
                return;
            }

            _stats = GamePlayModel.CharactersAttacking[_keyAttack];
            _rg = _stats.Transform.GetComponent<Rigidbody2D>();

            MoveToCharacterAttack();
        }

        private void MoveToCharacterAttack()
        {
            if (_stats.IsAttack)
            {
                return;
            }

            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPointX = _stats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f;
                var newPoint = new Vector3(newPointX, characterNearestPos.y);

                var velocity = (newPoint - _stats.Transform.position).normalized * CharacterConfig.speed;
                _rg.velocity = velocity;
            }
            else
            {
                _rg.velocity = _stats.Target.normalized * CharacterConfig.speed;
            }
        }

        private Character GetCharacterAttackNearest()
        {
            Character characterNearest = null;
            float minDistance = 10;
            foreach (var (_, characterStats) in GamePlayModel.CharactersAttacking)
            {
                if (_stats.Type == characterStats.Type || _stats.IsDeath || characterStats.IsDeath)
                {
                    continue;
                }

                var distance = Vector3.Distance(_stats.Transform.position, characterStats.Transform.position);
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