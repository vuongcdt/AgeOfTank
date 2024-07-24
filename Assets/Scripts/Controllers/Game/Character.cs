using System;
using System.Collections;
using Commands.Game;
using Cysharp.Threading.Tasks;
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
        private Character _characterTarget;
        private Character _characterStay;

        public CharacterStats Stats;

        protected override async void AwaitCustom()
        {
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
            if (!Stats.IsPlayer)
            {
                avatar.flipX = true;
            }

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

            if (_characterTarget)
            {
                return;
            }

            _characterTarget = other.GetComponent<Character>();

            transform.DOKill();

            this.SendCommand(new AttackCommand(_characterTarget, this));
        }

        private void SetCharacterDeath()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            transform.DOKill();
            // transform.position = new Vector3(5, 0);
            GamePlayModel.Characters.Remove(name);
            _characterTarget = null;
            SharedGameObjectPool.Return(gameObject);
        }

        private void SetSortingOrderHeathBar()
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - transform.position.y * 10);
        }

        private async void OnTriggerExit2D(Collider2D other)
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

            // if (!_characterTarget)
            // {
            //     return;
            // }
            //
            // var characterExit = other.GetComponent<Character>();
            // if (_characterTarget.Stats.ID != characterExit.Stats.ID)
            // {
            //     return;
            // }

            if (!_characterStay)
            {
                MoveToTarget();
                return;
            }

            if (!_characterStay.Stats.IsDeath)
            {
                this.SendCommand(new AttackCommand(_characterStay, this));
                return;
            }

            await NextAction();
        }

        // private void Update()
        // {
        //     var position = transform.position;
        //     var newPos = new Vector3(Stats.IsPlayer ? position.x + 0.5f : position.x - 0.5f, position.y - 3);
        //     Debug.DrawLine(newPos, newPos + Vector3.up * 6, Color.green);
        // }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other)
            {
                return;
            }

            var tagOpposition = Stats.IsPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            _characterStay = other.transform.GetComponent<Character>();
        }

        private async UniTask NextAction()
        {
            if (Stats.IsDeath)
            {
                return;
            }

            // await UniTask.WaitForSeconds(0.05f);
            await UniTask.WaitForEndOfFrame(this);

            var layerOpposition = Stats.IsPlayer ? layerEnemy : layerPlayer;
            var position = transform.position;
            var newPos = new Vector3(Stats.IsPlayer ? position.x + 0.5f : position.x - 0.5f, position.y - 3);
            var hit2D = Physics2D.Raycast(newPos, Vector3.up, 6, layerOpposition);

            if (!hit2D)
            {
                MoveToTarget();
                return;
            }

            transform.DOKill();
            // MoveToTarget();
            
            transform
                .DOMove(hit2D.collider.transform.position, _characterConfig.durationMove * 0.1f)
                .SetEase(Ease.Linear);
        }

        private async void MoveToOpposition()
        {
            await NextAction();
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