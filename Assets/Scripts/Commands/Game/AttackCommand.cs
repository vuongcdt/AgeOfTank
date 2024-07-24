using Controllers.Game;
using Cysharp.Threading.Tasks;

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
            await UniTask.WaitForSeconds(CharacterConfig.attackTime);

            if (!_characterTarget)
            {
                return;
            }

            if (_characterTarget.Stats.IsDeath || _characterAttack.Stats.IsDeath)
            {
                return;
            }

            _characterTarget.Stats.Health.Value -= _characterAttack.Stats.Damage;

            await AttackTarget();
        }
    }
}