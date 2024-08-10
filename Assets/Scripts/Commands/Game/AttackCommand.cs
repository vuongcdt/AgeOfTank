using Interfaces;

namespace Commands.Game
{
    public class AttackCommand : BaseCommand
    {
        private CharacterStats _statsBeaten;
        private CharacterStats _statsAttack;

        private string _keyBeaten;
        private string _keyAttack;

        public AttackCommand(string keyBeaten, string keyAttack)
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

            _statsBeaten.Health.Value -= _statsAttack.Damage;
        }
    }
}