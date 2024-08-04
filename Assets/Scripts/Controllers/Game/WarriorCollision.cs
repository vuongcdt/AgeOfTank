﻿using System;
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
            var isPlayer = _character.stats.Type == ENUMS.CharacterType.Player;
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
            _character.stats.CharactersCanBeaten.TryAdd(_characterBeaten.name, _characterBeaten);
            _character.stats.CharacterBeaten.Value = _characterBeaten;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!IsCompetitor(other.collider))
            {
                return;
            }
            
            _character.stats.CharacterBeaten.Value = null;
            _character.stats.CharactersCanBeaten.Remove(other.gameObject.name);
        }

        private bool IsCompetitor(Collider2D other)
        {
            var competitorTag = _character.stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
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