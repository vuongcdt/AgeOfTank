using System.Collections;
using DG.Tweening;
using Systems;
using UnityEngine;
using UnityEngine.UI;
using uPools;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private GameUIData gameUIData;
        [SerializeField] private GamePlayData gamePlayData;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;

        private Vector3 _source;
        private Vector3 _target;
        private CONSTANTS.CardCharacterType _type;
        private int _id;
        private bool _isPlayer;
        private Collider2D _collider;
        private float _health;
        private float _damage;
        private Character _characterTarget;

        public int ID => _id;
        public Vector3 Target => _target;
        public Vector3 Source => _source;
        public bool IsPlayer => _isPlayer;


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

            _health = gamePlayData.healths[(int)_type];
            _damage = gamePlayData.damages[(int)_type];

            healthBar.SetActive(false);
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

        private void SetSortingOrderHeathBar(Character characterTarget)
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - characterTarget.transform.position.y * 10);
        }

        private void SetCharacterDeath(Character character)
        {
            if (!character.gameObject.activeSelf)
            {
                return;
            }

            transform.DOKill();
            SharedGameObjectPool.Return(character.gameObject);
            _characterTarget = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = _isPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            transform.DOKill();

            var characterTarget = other.GetComponent<Character>();

            StartCoroutine(AttackIE(characterTarget));
        }

        private IEnumerator AttackIE(Character characterTarget)
        {
            _characterTarget = characterTarget;
            yield return new WaitForSeconds(gamePlayData.attackTime);

            if (!characterTarget)
            {
                yield break;
            }

            if (_health < 0)
            {
                yield break;
            }

            if (!characterTarget.gameObject.activeSelf)
            {
                yield break;
            }

            characterTarget.healthBar.SetActive(true);

            SetSortingOrderHeathBar(characterTarget);

            characterTarget._health -= _damage;
            characterTarget.healthSlider.value = characterTarget._health / gamePlayData.healths[(int)_type];
            
            if (characterTarget._health < 0)
            {
                SetCharacterDeath(characterTarget);
                yield break;
            }

            StartCoroutine(AttackIE(characterTarget));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_health < 0)
            {
                SetCharacterDeath(this);
                return;
            }

            if (!other || !gameObject.activeSelf)
            {
                return;
            }

            if (other.CompareTag(tag) || other.tag.Contains(CONSTANTS.Tag.CircleCollider) ||
                tag.Contains(CONSTANTS.Tag.CircleCollider))
            {
                return;
            }

            NextAction();
        }

        private void NextAction()
        {
            var hits = new RaycastHit2D[10];
            _collider.Cast(Vector2.right, hits, gamePlayData.distanceHit, true);

            var colliderTarget = GetTarget(hits);

            if (!colliderTarget)
            {
                var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                    transform.position.x, 
                    Source.x, 
                    Target.x,
                    gamePlayData.durationMove);
                
                transform
                    .DOMove(new Vector3(_target.x, transform.position.y), durationMoveToTarget)
                    .SetEase(Ease.Linear);
                return;
            }

            if (_characterTarget._health > 0)
            {
                return;
            }

            StartCoroutine(AttackIE(colliderTarget.GetComponent<Character>()));
        }

        private Collider2D GetTarget(RaycastHit2D[] hits)
        {
            foreach (var r in hits)
            {
                if (!r.collider)
                {
                    continue;
                }

                if (r.collider.gameObject.CompareTag(tag))
                {
                    continue;
                }

                if (!_characterTarget)
                {
                    continue;
                }

                if (r.collider.tag.Contains(CONSTANTS.Tag.CircleCollider))
                {
                    continue;
                }

                return r.collider;
            }

            return null;
        }
    }
}