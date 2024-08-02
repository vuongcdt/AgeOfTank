using Controllers.Game;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Commands.Game
{
    public class AttackCommand : BaseCommand
    {
        private Character _characterTarget;
        private Character _characterAttack;

        public AttackCommand(Character characterTarget, Character characterAttack)
        {
            _characterTarget = characterTarget;
            _characterAttack = characterAttack;
        }

        protected override async void OnExecute()
        {
            base.OnExecute();
            await AttackTarget();
        }

        private async UniTask AttackTarget()
        {
            await AttackAsync();
        }

        private async UniTask AttackAsync()
        {
            // _characterAttack.transform.DOKill();
            await UniTask.WaitForSeconds(ActorConfig.attackTime);
            if (!_characterTarget)
            {
                return;
            }

            if (_characterTarget.stats.IsDeath || _characterAttack.stats.IsDeath)
            {
                return;
            }

            _characterTarget.stats.Health.Value -= _characterAttack.stats.Damage;

            await AttackTarget();
        }
    }
}