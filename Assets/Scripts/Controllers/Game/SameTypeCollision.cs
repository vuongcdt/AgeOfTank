using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class SameTypeCollision : BaseGameController
    {
        [SerializeField] private int mass = 20;

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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(CONSTANS.Tag.PlayerBar))
            {
                var attackingCount = GamePlayModel.CharactersAttacking.Count;
                if (attackingCount > 0)
                {
                    _rg.mass = mass;
                    _rg.velocity = Vector3.zero;
                }

                return;
            }

            _character.MoveHead();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!_character.IsNearStartPoint())
            {
                // _rg.velocity = Vector3.zero;
                // _rg.AddForce(_character.Stats.Target.normalized * CharacterConfig.speed * 0.5f);
                _rg.velocity = _character.Stats.Target.normalized * CharacterConfig.speed * 0.5f;
            }
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