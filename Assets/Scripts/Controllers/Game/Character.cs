using System;
using System.Collections;
using DG.Tweening;
using Systems;
using UnityEngine;
using uPools;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private GameUIData gameUIData;
        [SerializeField] private GamePlayData gamePlayData;

        private Vector3 _source;
        private Vector3 _target;
        private CONSTANTS.CardCharacterType _type;
        private int _id;
        private bool _isPlayer;
        private Collider2D _collider;

        public int ID => _id;
        public Vector3 Target => _target;
        // public Vector3 Source => _source;

        protected override void AwaitCustom()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void InitCharacter(CONSTANTS.CardCharacterType type, int id)
        {
            _isPlayer = (int)type < 3;

            _type = type;
            _id = id;
            _source = _isPlayer ? gamePlayData.pointSource : gamePlayData.pointTarget;
            _target = !_isPlayer ? gamePlayData.pointSource : gamePlayData.pointTarget;
            avatar.sprite = gameUIData.imgAvatar[(int)type];
            tag = _isPlayer ? CONSTANTS.Tag.Player : CONSTANTS.Tag.Enemy;
            name = $"{tag} {id}";

            transform.position = _source;
            transform.DOKill();

            MoveCharacter();
        }

        private void MoveCharacter()
        {
            transform
                .DOMove(_target, gamePlayData.durationMove)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = _isPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            transform.DOKill();


            if (_isPlayer)
            {
                StartCoroutine(DisableIE());
            }
        }

        private IEnumerator DisableIE()
        {
            var random = Random.Range(3, 6);
            yield return new WaitForSeconds(random);
            transform.DOKill();
            SharedGameObjectPool.Return(gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other || !gameObject.activeSelf)
            {
                return;
            }

            if (other.CompareTag(tag) || other.tag.Contains(CONSTANTS.Tag.CircleCollider) ||
                tag.Contains(CONSTANTS.Tag.CircleCollider))
            {
                return;
            }

            MoveHead();
        }

        private void MoveHead()
        {
            var hits = new RaycastHit2D[3];
            _collider.Cast(Vector2.right, hits, gamePlayData.distanceHit, true);

            foreach (var r in hits)
            {
                if (!r.collider && !r.collider.gameObject.CompareTag(tag))
                {
                    return;
                }
            }

            transform
                .DOMove(new Vector3(_target.x, transform.position.y), gamePlayData.durationMove)
                .SetEase(Ease.Linear);
        }
    }
}