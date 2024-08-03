using System;
using DG.Tweening;
using Events;
using QFramework;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class WarriorCollider : BaseGameController
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
        
        private bool IsCompetitor(Collider2D other)
        {
            var competitorTag = _character.stats.Type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer;
            return other.CompareTag(competitorTag);
        }
        
        // private void OnCollisionEnter2D(Collision2D other)
        // {
        //     if (!IsCompetitor(other.collider))
        //     {
        //         return;
        //     }
        //
        //     _characterBeaten = other.collider.GetComponentInParent<Character>();
        //
        //     _character.stats.CharactersCanBeaten.TryAdd(_characterBeaten.name, _characterBeaten);
        //     _character.stats.CharacterBeaten.Value = _characterBeaten;
        // }
        //
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (!IsCompetitor(other))
        //     {
        //         return;
        //     }
        //
        //     _characterBeaten = other.GetComponentInParent<Character>();
        //
        //     _character.stats.CharactersCanBeaten.TryAdd(_characterBeaten.name, _characterBeaten);
        //     _character.stats.CharacterBeaten.Value = _characterBeaten;
        // }
        //
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (!IsCompetitor(other))
        //     {
        //         return;
        //     }
        //
        //     var characterExit = other.GetComponentInParent<Character>();
        //
        //     if (!characterExit)
        //     {
        //         return;
        //     }
        //
        //     if (_characterBeaten.stats.ID != characterExit.stats.ID)
        //     {
        //         return;
        //     }
        //
        //     _character.stats.CharactersCanBeaten.Remove(characterExit.name);
        //     MoveToCompetitor();
        // }
        //
        // private void MoveToCompetitor()
        // {
        //     if (!_character.gameObject.activeSelf)
        //     {
        //         return;
        //     }
        //
        //     var characterAttackNearest = CharacterAttackNearest();
        //
        //     if (!characterAttackNearest)
        //     {
        //         // _character.MoveToTarget();
        //         this.SendEvent<MoveToTargetEvent>();
        //         return;
        //     }
        //
        //     Vector3 characterAttackNearestPosition = characterAttackNearest.transform.position;
        //
        //     var random = Random.value * 0.1f;
        //     var durationMove = 6f;
        //
        //     var posCharacterX = characterAttackNearestPosition.x + (_character.stats.Type == ENUMS.CharacterType.Player
        //         ? -_circleCollider.radius * 2 + random
        //         : _circleCollider.radius * 2 - random);
        //
        //     _character.transform
        //         .DOMove(new Vector3(posCharacterX, characterAttackNearestPosition.y), durationMove)
        //         .OnComplete(MoveToCompetitor);
        // }
        //
        // private Character CharacterAttackNearest()
        // {
        //     Character characterAttackMin = null;
        //     var posCharacter = _character.transform.position;
        //     float minDistance = 10;
        //     foreach (var pair in GamePlayModel.CharactersAttacking)
        //     {
        //         var characterAttack = pair.Value;
        //         var posCharacterAttack = characterAttack.transform.position;
        //         var distance = Vector3.Distance(posCharacter, posCharacterAttack);
        //         if (!characterAttack.gameObject.activeSelf)
        //         {
        //             continue;
        //         }
        //
        //         if (distance < minDistance && characterAttack.stats.Type != _character.stats.Type)
        //         {
        //             minDistance = distance;
        //             // point = posCharacterAttack;
        //             characterAttackMin = characterAttack;
        //         }
        //     }
        //
        //     return characterAttackMin;
        // }

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