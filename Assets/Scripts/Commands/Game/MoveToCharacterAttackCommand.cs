using Controllers.Game;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class MoveToCharacterAttackCommand : BaseCommand
    {
        private string _characterName;
        private Rigidbody2D _rg;

        private CharacterStats _characterStats;
        private Animator _animator;

        private static readonly int WalkAnimator = Animator.StringToHash("walk");
        
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
            _animator = _characterStats.Transform.GetComponentInChildren<Animator>();
            MoveToCharacterAttack();
        }

        private void MoveToCharacterAttack()
        {
            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                if (_characterStats.IsAttackCharacter || _characterStats.IsAttackTarget)
                {
                    return;
                }
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPointX = _characterStats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f;
                var newPoint = new Vector3(newPointX, characterNearestPos.y);

                var velocity = (newPoint - _characterStats.Transform.position).normalized * CharacterConfig.speed;
                _rg.mass = 1;
                _rg.velocity = velocity;
                // _rg.AddForce( velocity * 0.5f);
            }
            else
            {
                _rg.velocity = _characterStats.Target.normalized * CharacterConfig.speed;
            }
            
            _animator.SetTrigger(WalkAnimator);
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