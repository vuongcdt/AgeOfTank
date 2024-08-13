using Commands.Game;
using QFramework;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

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

            this.SendCommand(new MoveToCharacterAttackCommand(_character.name));
        }

        private async void OnTriggerExit2D(Collider2D other)
        {
            if (IsCompareStartBarTag(other))
            {
                var random = Random.value * 2;
                await UniTask.WaitForSeconds(random);
                
                _rg.mass = 1;
                _capsuleCollider.isTrigger = false;
            }

            var characterHead = other.GetComponent<Character>();
            if (!characterHead)
            {
                return;
            }

            var isCharacterBehind = _character.Stats.ID > characterHead.Stats.ID;
            if (_character.IsNearStartPoint() && other.CompareTag(tag) && isCharacterBehind)
            {
                var characterHeadPos = other.transform.position;
                var characterTransform = _character.transform;
                var random = (0.5f - Random.value) * 0.05f;
                characterTransform.position = new Vector3(characterHeadPos.x + random, characterTransform.position.y);
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