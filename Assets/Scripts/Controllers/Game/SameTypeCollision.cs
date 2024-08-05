using System.Collections;
using Interfaces;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class SameTypeCollision : BaseGameController
    {
        [SerializeField] private int mass = 20;
        private CharacterStats _characterStats;
        private Character _character;
        private Rigidbody2D _rg;
        private CapsuleCollider2D _capsuleCollider;
        private bool _isStopMove;

        private void Start()
        {
            _rg = GetComponent<Rigidbody2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _characterStats = GamePlayModel.Characters[name];
            _character = GetComponent<Character>();
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
                    _isStopMove = true;
                }
                return;
            }
            
            if (_characterStats.IsAttack)
            {
                return;
            }
            StartCoroutine(AddVelocityIE());
        }

        private IEnumerator AddVelocityIE()
        {
            if (_isStopMove)
            {
                yield break;
            }
            var magnitude = _rg.velocity.magnitude;

            if (magnitude < 0.2f && !_characterStats.IsAttack)
            {
                _rg.AddForce(_characterStats.Target.normalized * 0.5f);
            }

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(AddVelocityIE());
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!_character.IsNearStartPoint())
            {
                _rg.velocity = Vector3.zero;
                _rg.AddForce(_characterStats.Target.normalized * 0.2f);
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
            _characterStats = GamePlayModel.Characters[name];
            if (_characterStats is null)
            {
                return false;
            }

            var startBarTag = _characterStats.IsPlayer ? CONSTANS.Tag.StartBarPlayer : CONSTANS.Tag.StartBarEnemy;
            return other.CompareTag(startBarTag);
        }
    }
}