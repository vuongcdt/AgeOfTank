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
        private CancellationTokenSource _cancelAttackCharacter = new();

        public AttackTargetCommand(string keyAttack)
        {
            _keyAttack = keyAttack;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
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

            _statsAttack = GamePlayModel.Characters[_keyAttack];

            if (_statsAttack.IsAttackCharacter)
            {
                _statsAttack.IsAttackTarget = false;
                return;
            }

            Debug.Log($"AttackTarget {_statsAttack.Name}");
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