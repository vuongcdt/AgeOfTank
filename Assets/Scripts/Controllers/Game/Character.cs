﻿using System.Collections.Generic;
using Commands.Game;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Interfaces;
using QFramework;
using Systems;
using UnityEngine;
using UnityEngine.UI;
using uPools;
using Utilities;

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

        private Character _characterTarget;
        private Character _characterStay;
        private Dictionary<string, Character> _charEnterDictionary = new();

        public bool IsMoveTarget => isMoveTarget;

        public CharacterStats Stats;

        public void InitCharacter(string key)
        {
            var idText = GetComponentInChildren<TextMesh>();
            Stats = GamePlayModel.Characters[key];
            avatar.sprite = ActorConfig.unitConfigs[(int)Stats.TypeClass].imgAvatar;
            tag = Stats.Tag;
            name = Stats.Name;
            idText.text = Stats.ID.ToString();
            idText.transform.localPosition = Stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            gameObject.layer = Stats.IsPlayer ? (int)ENUMS.Layer.Player : (int)ENUMS.Layer.Enemy;
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

                healthSlider.value = newValue / ActorConfig.unitConfigs[(int)Stats.TypeClass].health;
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
                ActorConfig.durationMove);

            transform
                .DOMove(new Vector3(Stats.Target.x, position.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }

        public void MoveNewPoint(Vector3 newPosition, float time = 0)
        {
            if (time == 0)
            {
                time = ActorConfig.durationMove * 0.1f;
            }

            transform.DOKill();
            transform
                .DOMove(newPosition, time)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = Stats.IsPlayer ? CONSTANS.Tag.Enemy : CONSTANS.Tag.Player;

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
            var tagOpposition = Stats.IsPlayer ? CONSTANS.Tag.Enemy : CONSTANS.Tag.Player;

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

        private async void NextAction()
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
                isMoveTarget = true;
                enabled = false;
                await UniTask.WaitForEndOfFrame(this);
                enabled = true;
                MoveToTarget();
                return;
            }

            var durationMove = 0.5f / minDistance * (ActorConfig.durationMove * 0.1f);
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
    }
}