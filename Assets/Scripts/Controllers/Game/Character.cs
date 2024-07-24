using System;
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
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private LayerMask layerPlayer;
        [SerializeField] private LayerMask layerEnemy;

        private CharacterConfig _characterConfig;
        private Collider2D _collider;
        private Character _characterTarget;

        public CharacterStats Stats;

        protected override async void AwaitCustom()
        {
            _collider = GetComponent<Collider2D>();
            _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        }

        public void InitCharacter(string key)
        {
            Stats = GamePlayModel.Characters[key];
            avatar.sprite = _characterConfig.unitConfigs[(int)Stats.Type].imgAvatar;
            tag = Stats.Tag;
            name = Stats.Name;
            gameObject.layer = Stats.IsPlayer ? (int)CONSTANTS.Layer.Player : (int)CONSTANTS.Layer.Enemy;
            healthBar.SetActive(false);
            transform.position = Stats.Source;
            transform.DOKill();

            Stats.Health.Register(newValue =>
            {
                if (newValue <= 0)
                {
                    SetCharacterDeath();
                    return;
                }

                healthBar.SetActive(true);
                SetSortingOrderHeathBar();

                healthSlider.value = newValue / _characterConfig.unitConfigs[(int)Stats.Type].health;
            });

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            var position = transform.position;
            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                position.x,
                Stats.Source.x,
                Stats.Target.x,
                _characterConfig.durationMove);

            transform
                .DOMove(new Vector3(Stats.Target.x, position.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = Stats.IsPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            if (_characterTarget && !_characterTarget.Stats.IsDeath)
            {
                return;
            }

            transform.DOKill();

            _characterTarget = other.GetComponent<Character>();

            this.SendCommand(new AttackCommand(_characterTarget, this));
        }

        private void SetCharacterDeath()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            transform.DOKill();
            GamePlayModel.Characters.Remove(name);
            _characterTarget = null;
            SharedGameObjectPool.Return(gameObject);
        }

        private void SetSortingOrderHeathBar()
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - transform.position.y * 10);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other)
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

        private void Update()
        {
            var position = transform.position;
            Debug.DrawLine(position, position + (Stats.IsPlayer ? Vector3.right : Vector3.left), Color.green);
        }

        private void NextAction()
        {
            if (Stats.IsDeath)
            {
                return;
            }

            // var hits = new RaycastHit2D[10];
            // _collider.Cast(Vector2.right, hits, _characterConfig.distanceHit, true);
            //
            // var colliderTarget = GetTarget(hits);
            var layerOpposition = Stats.IsPlayer ? layerEnemy : layerPlayer;
            var position = transform.position;
            var hit2D = Physics2D.Raycast(position, Stats.IsPlayer ? Vector3.right : Vector3.left,
                1,
                layerOpposition);
            // Debug.Log($"{name} {hit2D.collider.name}");
            
            // var collider2Ds = new Collider2D[5];
            // var size = Physics2D.OverlapCircleNonAlloc(position, 3, collider2Ds, layerOpposition);

            if (!hit2D)
            {
                MoveToTarget();
                return;
            }

            if (!_characterTarget.Stats.IsDeath)
            {
                return;
            }

            var characterTarget = hit2D.collider.gameObject.GetComponent<Character>();
            this.SendCommand(new AttackCommand(characterTarget, this));
        }

        private Collider2D GetTarget(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (!hit.collider)
                {
                    continue;
                }

                if (hit.collider.gameObject.CompareTag(tag))
                {
                    continue;
                }

                if (!_characterTarget)
                {
                    continue;
                }

                if (hit.collider.tag.Contains(CONSTANTS.Tag.CircleCollider))
                {
                    continue;
                }

                return hit.collider;
            }

            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var circleCollider = GetComponent<CircleCollider2D>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
#endif
    }
}