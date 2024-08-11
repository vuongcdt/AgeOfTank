using System.Collections;
using Commands.Game;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class WarriorCollision : BaseGameController
    {
        private Character _character;
        private Character _characterBeaten;

        private void Start()
        {
            _character = GetComponentInParent<Character>();
        }

        public void SetTagAndLayer(ENUMS.CharacterType type)
        {
            var isPlayer = type == ENUMS.CharacterType.Player;
            tag = isPlayer
                ? CONSTANS.Tag.WarriorColliderPlayer
                : CONSTANS.Tag.WarriorColliderEnemy;

            gameObject.layer = isPlayer
                ? (int)ENUMS.Layer.WarriorPlayer
                : (int)ENUMS.Layer.WarriorEnemy;

            // gameObject.layer = isPlayer
            // ? LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorPlayer)
            // : LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorEnemy);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsTargetCompetitor(other.collider))
            {
                this.SendCommand(new AttackTargetCommand(_character.name));
                return;
            }

            if (!IsCharacterCompetitor(other.collider))
            {
                return;
            }

            _characterBeaten = other.collider.GetComponentInParent<Character>();
            if (!_characterBeaten)
            {
                return;
            }

            _character.AttackCharacter(_characterBeaten.name);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsCharacterCompetitor(other))
            {
                return;
            }

            if (_character.Stats.IsDeath)
            {
                return;
            }

            var characterExit = other.GetComponentInParent<Character>();

            if (characterExit && _characterBeaten && _characterBeaten.Stats.ID != characterExit.Stats.ID)
            {
                return;
            }

            _character.MoveHead();
        }

        private bool IsCharacterCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
        }

        private bool IsTargetCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.TargetPlayer
                : CONSTANS.Tag.TargetEnemy;
            return other.CompareTag(competitorTag);
        }
    }
}