using System.Collections;
using Commands.Game;
using DG.Tweening;
using Interfaces;
using QFramework;
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

        private Collider2D _collider;
        private Character _characterTarget;
        private IEnumerator _attackIE;

        public CharacterModel Model;

        protected override void AwaitCustom()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void InitCharacter(CONSTANTS.CardCharacterType type, int id)
        {
            var isPlayer = (int)type < 3;
            var source = isPlayer ? gamePlayData.pointSource : gamePlayData.pointTarget;
            var target = !isPlayer ? gamePlayData.pointSource : gamePlayData.pointTarget;
            var health = gamePlayData.healths[(int)type];
            var damage = gamePlayData.damages[(int)type];

            avatar.sprite = gameUIData.imgAvatar[(int)type];
            tag = isPlayer ? CONSTANTS.Tag.Player : CONSTANTS.Tag.Enemy;
            name = $"{tag} {id}";

            healthBar.SetActive(false);
            transform.position = source;
            transform.DOKill();


            GamePlayModel.Characters.Add(name, new CharacterModel(health, id, damage, target, source, type));

            Model = GamePlayModel.Characters[name];
            Model.Health.RegisterWithInitValue(newValue =>
            {
                healthSlider.value = newValue / gamePlayData.healths[(int)type];
            });

            MoveCharacter();
        }

        private void MoveCharacter()
        {
            transform
                .DOMove(Model.Target, gamePlayData.durationMove)
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

            if (_attackIE != null)
            {
                StopCoroutine(_attackIE);
            }

            _attackIE = null;
            transform.DOKill();
            GamePlayModel.Characters.Remove(name);
            _characterTarget = null;
            SharedGameObjectPool.Return(character.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = Model.IsPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            transform.DOKill();

            var characterTarget = other.GetComponent<Character>();

            AttackTarget(characterTarget);
        }

        private void AttackTarget(Character characterTarget)
        {
            if (_attackIE != null)
            {
                StopCoroutine(_attackIE);
            }

            _attackIE = AttackIE(characterTarget);
            StartCoroutine(_attackIE);
        }

        private IEnumerator AttackIE(Character characterTarget)
        {
            _characterTarget = characterTarget;
            yield return new WaitForSeconds(gamePlayData.attackTime);

            if (!characterTarget)
            {
                yield break;
            }

            if (Model.IsDeath)
            {
                yield break;
            }

            if (!characterTarget.gameObject.activeSelf)
            {
                yield break;
            }

            characterTarget.healthBar.SetActive(true);

            SetSortingOrderHeathBar(characterTarget);

            characterTarget.Model.Health.Value -= Model.Damage; // attack
            // this.SendCommand(new AttackCommand(characterTarget,this));

            if (characterTarget.Model.IsDeath)
            {
                SetCharacterDeath(characterTarget);
                yield break;
            }

            AttackTarget(characterTarget);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Model.IsDeath)
            {
                SetCharacterDeath(this);
                return;
            }

            if (!other || !gameObject.activeSelf)
            {
                return;
            }

            if (other.CompareTag(tag) || 
                other.tag.Contains(CONSTANTS.Tag.CircleCollider) ||
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
                    Model.Source.x,
                    Model.Target.x,
                    gamePlayData.durationMove);

                transform
                    .DOMove(new Vector3(Model.Target.x, transform.position.y), durationMoveToTarget)
                    .SetEase(Ease.Linear);
                return;
            }

            if (!_characterTarget.Model.IsDeath)
            {
                return;
            }

            var characterTarget = colliderTarget.GetComponent<Character>();
            AttackTarget(characterTarget);
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