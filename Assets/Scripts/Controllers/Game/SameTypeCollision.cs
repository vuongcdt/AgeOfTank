using System.Collections;
using Interfaces;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class SameTypeCollision : BaseGameController
    {
        private CharacterStats _characterStats;
        private Rigidbody2D _rg;
        private BoxCollider2D _boxCollider;
        private CapsuleCollider2D _capsuleCollider;

        private void Start()
        {
            _rg = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _characterStats = GamePlayModel.Characters[name];
            // _characterStats = GetComponent<Character>().stats;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_characterStats.IsAttack)
            {
                return;
            }

            StartCoroutine(AddVelocityIE());
        }

        private IEnumerator AddVelocityIE()
        {
            var magnitude = _rg.velocity.magnitude;

            if (magnitude < 0.2f && !_characterStats.IsAttack)
            {
                // _rg.velocity = stats.Target.normalized * 0.2f;
                // _rg.AddForce((_targetMovePoint - position).normalized * 0.2f);
                _rg.AddForce(_characterStats.Target.normalized * 0.2f);
            }

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(AddVelocityIE());
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            Debug.Log($"OnCollisionExit2D {name}");
            // _rg.velocity = _characterStats.Target.normalized * 0.2f;
            _rg.velocity = Vector3.zero;
            _rg.AddForce(_characterStats.Target.normalized * 0.2f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsCompareStartBarTag(other))
            {
                _rg.mass = 1;
                // _boxCollider.isTrigger = false;
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider2D>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
#endif
    }
}