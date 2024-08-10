using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class HunterCollider : MonoBehaviour
    {
        private Character _character;

        public void Init(Character character)
        {
            _character = character;
            var isHunterClass = (int)_character.Stats.TypeClass % 3 == 1;
            gameObject.SetActive(isHunterClass);

            tag = CONSTANS.Tag.HunterCollider;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_character)
            {
                return;
            }
            var characterBeaten = other.GetComponentInParent<Character>();

            // var opposingTag = _character.Stats.Type == ENUMS.CharacterType.Player
            //     ? CONSTANS.Tag.WarriorColliderEnemy
            //     : CONSTANS.Tag.WarriorColliderPlayer;
            var opposingTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;

            if (other.CompareTag(opposingTag))
            {
                _character.Attack(characterBeaten);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<CircleCollider2D>();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
#endif
    }
}