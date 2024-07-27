using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class WarriorCollider : MonoBehaviour
    {
        private Actor _actor;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _actor = GetComponentInParent<Actor>();
            tag = _actor.type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderPlayer
                : CONSTANS.Tag.WarriorColliderEnemy;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(_actor.type == ENUMS.CharacterType.Player
                    ? CONSTANS.Tag.WarriorColliderEnemy
                    : CONSTANS.Tag.WarriorColliderPlayer))
            {
                _actor.Attack();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<CircleCollider2D>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
#endif
    }
}