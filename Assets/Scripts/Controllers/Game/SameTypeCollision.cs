using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class SameTypeCollision : BaseGameController
    {
        private Character _character;
        private Rigidbody2D _rg;
        private CapsuleCollider2D _capsuleCollider;

        private void Start()
        {
            _rg = GetComponent<Rigidbody2D>();
            _character = GetComponent<Character>();
        }

        private void OnEnable()
        {
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _capsuleCollider.isTrigger = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (_character.IsNearStartPoint())
            {
                return;
            }
            _character.MoveToTarget();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsCompareStartBarTag(other))
            {
                _rg.mass = 1;
                _capsuleCollider.isTrigger = false;
            }
        }

        private bool IsCompareStartBarTag(Collider2D other)
        {
            if (_character.Stats is null)
            {
                return false;
            }

            var startBarTag = _character.Stats.IsPlayer ? CONSTANS.Tag.StartBarPlayer : CONSTANS.Tag.StartBarEnemy;
            return other.CompareTag(startBarTag);
        }
    }
}