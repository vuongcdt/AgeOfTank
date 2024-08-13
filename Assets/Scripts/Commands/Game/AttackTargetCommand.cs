using System.Threading;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Commands.Game
{
    public class AttackTargetCommand : BaseCommand
    {
        private string _keyAttack;
        private CharacterStats _statsAttack;
        private Rigidbody2D _rg;

        public AttackTargetCommand(string keyAttack)
        {
            _keyAttack = keyAttack;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            
            _statsAttack = GamePlayModel.Characters[_keyAttack];
            _rg = _statsAttack.Transform.GetComponent<Rigidbody2D>();
            _rg.mass = CharacterConfig.mass;
            _rg.velocity = Vector3.zero;
            
            AttackTarget();
        }

        private async void AttackTarget()
        {
            await UniTask.WaitForSeconds(CharacterConfig.attackTime);
            var isCharacterAttack = GamePlayModel.Characters.ContainsKey(_keyAttack);

            if (!isCharacterAttack)
            {
                return;
            }
            
            if (_statsAttack.IsAttackCharacter)
            {
                _statsAttack.IsAttackTarget = false;
                return;
            }

            _statsAttack.IsAttackTarget = true;

            if (_statsAttack.IsPlayer)
            {
                GamePlayModel.HealthTargetPlayer.Value -= _statsAttack.Damage;
            }
            else
            {
                GamePlayModel.HealthTargetEnemy.Value -= _statsAttack.Damage;
            }

            AttackTarget();
        }
    }
}