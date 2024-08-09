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
                ? LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorPlayer)
                : LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorEnemy);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsCompetitor(other.collider))
            {
                return;
            }

            _characterBeaten = other.collider.GetComponentInParent<Character>();

            if (_character.IsNearStartPoint() || _characterBeaten.IsNearStartPoint())
            {
                return;
            }

            _character.Attack(_characterBeaten);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsCompetitor(other))
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

        private bool IsCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
        }
    }
}