using Interfaces;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class SameTypeCollider : BaseGameController
    {
        private Character _character;
        private CharacterStats _characterStats;

        private string _characterName;

        public string ActorName => _characterName;

        private void Start()
        {
            _character = GetComponentInParent<Character>();
            _characterName = _character.name;
            _characterStats = GamePlayModel.Characters[_characterName];
        }

        //OnTriggerEnter2D
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GetStartBarTag()))
            {
                MovetoOverStartBar();
                return;
            }

            if (!IsSameTag(other) || _characterStats.IsAttack)
            {
                return;
            }

            var characterObstacle = other.GetComponentInParent<Character>();
            if (_characterStats.ID <= characterObstacle.Stats.ID)
            {
                return;
            }

            _characterStats.CharacterObstacle.Value = characterObstacle;
            if (_character.IsNearStartPoint())
            {
                return;
            }

            // if (_characterStats.CharactersHead.Count > 2 && !_character.IsNearStartPoint())
            // {
            //     Debug.Log($"IsNearStartPoint {_characterStats.CharactersHead.Count}");
            //     _character.transform.DOKill();
            //     return;
            // }

            // MoveAcross();
            _characterStats.IsEnterSameTypeCollider.Value = true;
        }

        private void MovetoOverStartBar()
        {
            // if (_characterStats.CharactersHead.Count > 2 && !_character.IsNearStartPoint())
            // {
            //     _character.transform.DOKill();
            //     return;
            // }

            if (!_characterStats.CharacterObstacle.Value)
            {
                // _characterRun.MoveToTarget();
                _characterStats.IsEnterSameTypeCollider.Value = false;
                return;
            }

            // MoveAcross();
            _characterStats.IsEnterSameTypeCollider.Value = true;
        }

        private string GetStartBarTag()
        {
            return _characterStats.IsPlayer ? CONSTANS.Tag.StartBarPlayer : CONSTANS.Tag.StartBarEnemy;
        }

        //OnTriggerStay2D
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsSameTag(other) || _characterStats.IsAttack)
            {
                return;
            }

            var characterStay = other.GetComponentInParent<Character>();
            if (characterStay.Stats.ID > _characterStats.ID)
            {
                return;
            }

            _characterStats.CharacterObstacle.Value = characterStay;
        }

        private bool IsSameTag(Collider2D other)
        {
            return other.CompareTag(tag)
                   && tag.Contains(CONSTANS.Tag.SameTypeCollider);
        }

        //OnTriggerExit2D
        private void OnTriggerExit2D(Collider2D other)
        {
            if (_characterStats.IsAttack || !_characterStats.CharacterObstacle.Value ||
                !other.CompareTag(tag))
            {
                return;
            }

            var characterExit = other.GetComponentInParent<Character>();

            if (!characterExit)
            {
                return;
            }

            if (_characterStats.CharacterObstacle.Value.Stats.ID != characterExit.Stats.ID)
            {
                return;
            }

            // _characterRun.MoveToTarget();
            _characterStats.IsEnterSameTypeCollider.Value = false;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider2D>();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
#endif
    }
}