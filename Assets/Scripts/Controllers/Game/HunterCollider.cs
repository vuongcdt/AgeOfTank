using Commands.Game;
using QFramework;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class HunterCollider : BaseGameController
    {
        private Character _character;

        private void Start()
        {
            _character = GetComponentInParent<Character>();
            tag = CONSTANS.Tag.HunterCollider;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_character)
            {
                return;
            }

            var characterBeaten = other.GetComponentInParent<Character>();

            var opposingTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;

            if (other.CompareTag(opposingTag))
            {
                this.SendCommand(new AttackCharacterCommand(characterBeaten.name, _character.name));
            }

            var targetTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.TargetPlayer
                : CONSTANS.Tag.TargetEnemy;
            if (other.CompareTag(targetTag))
            {
                this.SendCommand(new AttackTargetCommand(_character.name));
            }
        }
    }
}