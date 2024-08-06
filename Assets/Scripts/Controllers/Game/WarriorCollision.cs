using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class WarriorCollision : BaseGameController
    {
        private Character _character;
        private Character _characterBeaten;
        private CircleCollider2D _circleCollider;
        public CircleCollider2D CircleCollider => _circleCollider;

        protected override void AwakeCustom()
        {
            base.AwakeCustom();
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _character = GetComponentInParent<Character>();
            var isPlayer = _character.Stats.Type == ENUMS.CharacterType.Player;
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

            _character.IsAttack = false;
            _character.MoveHead(0.05f);
        }

        private bool IsCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
        }

// #if UNITY_EDITOR
//         private void OnDrawGizmos()
//         {
//             var col = GetComponent<CircleCollider2D>();
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireSphere(transform.position, col.radius);
//         }
// #endif
    }
}