using System;
using DG.Tweening;
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

        private void Start()
        {
            _rg = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _characterStats = GamePlayModel.Characters[name];
            // _characterStats = GetComponent<Character>().stats;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsCompareTag(other))
            {
                Debug.Log($"OnTriggerEnter2D {name}");
                transform.DOKill();
                _rg.mass = 1;
                _boxCollider.isTrigger = false;
            }
         
        }

        private bool IsCompareTag(Collider2D other)
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