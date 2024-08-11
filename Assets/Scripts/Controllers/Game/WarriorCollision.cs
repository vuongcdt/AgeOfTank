﻿using System.Collections;
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
                ? (int)ENUMS.Layer.WarriorPlayer 
                : (int)ENUMS.Layer.WarriorEnemy;
            
            // gameObject.layer = isPlayer
            // ? LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorPlayer)
            // : LayerMask.NameToLayer(CONSTANS.LayerMask.WarriorEnemy);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsTargetCompetitor(other.collider))
            {
                StartCoroutine(AttackTarget());
                return;
            }
            if (!IsCharacterCompetitor(other.collider))
            {
                return;
            }

            _characterBeaten = other.collider.GetComponentInParent<Character>();
            if (!_characterBeaten)
            {
                return;
            }

            _character.AttackCharacter(_characterBeaten);
        }

        private IEnumerator AttackTarget()
        {
            yield return new WaitForSeconds(CharacterConfig.attackTime);
            if (_character.Stats.IsPlayer)
            {
                GamePlayModel.HealthTargetPlayer.Value -= _character.Stats.Damage;
            }
            else
            {
                GamePlayModel.HealthTargetEnemy.Value -= _character.Stats.Damage;
            }
            StartCoroutine(AttackTarget());
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsCharacterCompetitor(other))
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

        private bool IsCharacterCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
        }
        private bool IsTargetCompetitor(Collider2D other)
        {
            var competitorTag = _character.Stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.TargetPlayer
                : CONSTANS.Tag.TargetEnemy;
            return other.CompareTag(competitorTag);
        }
    }
}