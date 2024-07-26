using System.Collections.Generic;
using Commands.Game;
using DG.Tweening;
using Interfaces;
using QFramework;
using Systems;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private bool isGreenLine;
        [SerializeField] private bool isMoveTarget;

        private CharacterConfig _characterConfig;
        private Character _characterTarget;
        private Character _characterStay;
        private Dictionary<string, Character> _charEnterDictionary = new();

        public bool IsMoveTarget => isMoveTarget;

        public CharacterStats Stats;

        protected override async void AwaitCustom()
        {
            _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        }

        public void InitCharacter(string key)
        {
            var idText = GetComponentInChildren<TextMesh>();
            Stats = GamePlayModel.Characters[key];
            avatar.sprite = _characterConfig.unitConfigs[(int)Stats.Type].imgAvatar;
            tag = Stats.Tag;
            name = Stats.Name;
            idText.text = Stats.ID.ToString();

            gameObject.layer = Stats.IsPlayer ? (int)CONSTANTS.Layer.Player : (int)CONSTANTS.Layer.Enemy;
            healthBar.SetActive(false);
            transform.position = Stats.Source;
            transform.DOKill();
            avatar.flipX = !Stats.IsPlayer;

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

        public void MoveToTarget()
        {
            if (Stats.IsDeath)
            {
                return;
            }

            isMoveTarget = true;
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

        public void MoveNewPoint(Vector3 newPosition, float time = 0)
        {
            if (time == 0)
            {
                time = _characterConfig.durationMove * 0.1f;
            }

            transform.DOKill();
            transform
                .DOMove(newPosition, time)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = Stats.IsPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition))
            {
                return;
            }

            var characterEnter = other.GetComponent<Character>();
            if (characterEnter)
            {
                _charEnterDictionary.TryAdd(characterEnter.name, characterEnter);
            }

            if (_characterTarget && !_characterTarget.Stats.IsDeath)
            {
                return;
            }

            _characterTarget = characterEnter;

            transform.DOKill();

            this.SendCommand(new AttackCommand(characterEnter, this));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var tagOpposition = Stats.IsPlayer ? CONSTANTS.Tag.Enemy : CONSTANTS.Tag.Player;

            if (!other.CompareTag(tagOpposition) || Stats.IsDeath)
            {
                return;
            }

            transform.DOKill();
            var characterExit = other.GetComponent<Character>();

            if (characterExit && _charEnterDictionary.ContainsKey(characterExit.name))
            {
                _charEnterDictionary.Remove(characterExit.name);
            }

            if (!_characterTarget.Stats.IsDeath)
            {
                return;
            }

            foreach (var pair in _charEnterDictionary)
            {
                if (!_charEnterDictionary[pair.Key].Stats.IsDeath)
                {
                    _characterTarget = _charEnterDictionary[pair.Key];
                    this.SendCommand(new AttackCommand(_characterTarget, this));
                    return;
                }
            }

            NextAction();
        }

        private void NextAction()
        {
            if (Stats.IsDeath)
            {
                return;
            }

            var layerOpposition = Stats.IsPlayer ? layerEnemy : layerPlayer;
            var position = transform.position;
            var newPos = new Vector3(Stats.IsPlayer ? position.x + 0.5f : position.x - 0.5f, position.y - 3);

            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(position, 2, layerOpposition);

            isMoveTarget = false;
            Vector3 pointMinDistance = new Vector3();
            float minDistance = 10;

            foreach (var col2D in collider2Ds)
            {
                if (!col2D)
                {
                    continue;
                }

                var distance = Vector3.Distance(col2D.transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    pointMinDistance = col2D.transform.position;
                }
            }

            if (collider2Ds.Length == 0)
            {
                MoveToTarget();
                return;
            }

            var durationMove = 0.5f / minDistance * (_characterConfig.durationMove * 0.1f);
            var newPoint = new Vector3(Stats.IsPlayer ? pointMinDistance.x - 0.6f : pointMinDistance.x + 0.6f,
                pointMinDistance.y);
            MoveNewPoint(newPoint, durationMove);
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


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var circleCollider = GetComponent<CircleCollider2D>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
            if (!isGreenLine)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
#endif
    }
}