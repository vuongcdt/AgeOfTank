using Controllers.Game;

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

        protected override void OnExecute()
        {
            base.OnExecute();
            _characterTarget.Model.Health.Value -= _characterAttack.Model.Damage;
        }
    }
}