using Interfaces;
using QFramework;
using UnityEngine;
using uPools;

namespace Commands.Game
{
    public class SetCharacterDeathCommand : BaseCommand
    {
        private string _characterName;
        private Rigidbody2D _rg;

        private CharacterStats _characterStats;
        private Animator _animator;
        private static readonly int DieAnimator = Animator.StringToHash("die");

        public SetCharacterDeathCommand(string characterName)
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
            SetCharacterDeath();
        }

        private void SetCharacterDeath()
        {
            _animator.SetTrigger(DieAnimator);
            _rg.mass = 1;
            _characterStats.IsAttackCharacter = false;
            _characterStats.CharactersCanBeaten.Clear();
            GamePlayModel.CharactersAttacking.Remove(_characterName);
            GamePlayModel.Characters.Remove(_characterName);

            bool isHasCharacter = false;

            foreach (var (_, characterStats) in GamePlayModel.CharactersAttacking)
            {
                if (characterStats.Type == _characterStats.Type)
                {
                    isHasCharacter = true;
                }
            }

            if (!isHasCharacter)
            {
                this.SendEvent<MoveHeadEvent>();
                // this.SendEvent(new InitCharacter(_stats.TypeClass));
            }

            SharedGameObjectPool.Return(_characterStats.Transform.gameObject);
        }
    }
}