﻿using System;
using System.Collections;
using Commands.Game;
using Interfaces;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using uPools;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        public CharacterStats Stats => _stats;

        [SerializeField] private int mass = 20;
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private GameObject hunterCollider;

        private Rigidbody2D _rg;
        private CharacterStats _stats;
        [SerializeField] private string _keyBeaten;
        private IEnumerator _moveToPointIE;
        private IEnumerator _attackCharacterIE;
        private Move _move;

        private void OnDisable()
        {
            if (_moveToPointIE != null)
            {
                StopCoroutine(_moveToPointIE);
            }

            if (_attackCharacterIE != null)
            {
                StopCoroutine(_attackCharacterIE);
            }
        }

        public bool IsNearStartPoint()
        {
            var isNearStartPoint = Math.Abs(transform.position.x + Stats.Source.x) >
                                   Mathf.Abs(Stats.Source.x - Stats.Target.x) - 0.8f;
            return isNearStartPoint;
        }

        public void RenderCharacter(string key)
        {
            _stats = GamePlayModel.Characters[key];

            var warriorCollision = GetComponentInChildren<WarriorCollision>();
            warriorCollision.SetTagAndLayer(_stats.Type);

            var isHunterClass = (int)_stats.TypeClass % 3 == 1;
            hunterCollider.SetActive(isHunterClass);
            gameObject.layer = isHunterClass ? (int)ENUMS.Layer.SameTypeHunter : (int)ENUMS.Layer.SameType;

            var idText = GetComponentInChildren<TextMesh>();
            avatar.sprite = CharacterConfig.unitConfigs[(int)_stats.TypeClass].imgAvatar;
            tag = _stats.Tag;
            name = _stats.Name;
            idText.text = _stats.ID.ToString();
            idText.transform.localPosition = _stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            healthBar.SetActive(false);
            var random = (1 - Random.value) * 0.2f;
            var transform1 = transform;

            transform1.position = new Vector3(_stats.Source.x, _stats.Source.y + random);
            avatar.flipX = !_stats.IsPlayer;
            _stats.Transform = transform1;
            Init();
        }

        private void Init()
        {
            _stats.Health.Register(SetHealthBar);

            this.RegisterEvent<MoveHeadEvent>(e => this.SendCommand(new MoveToCharacterAttackCommand(name)));

            _rg = GetComponent<Rigidbody2D>();
            MoveHead(CharacterConfig.speed);
        }

        public void MoveHead(float speed)
        {
            _rg.velocity = _stats.Target.normalized * speed;
        }

        private void SetHealthBar(float newValue)
        {
            if (newValue <= 0)
            {
                // SetCharacterDeath();
                this.SendCommand(new SetCharacterDeathCommand(name));
                return;
            }

            healthBar.SetActive(true);
            SetSortingOrderHeathBar();

            healthSlider.value = newValue / CharacterConfig.unitConfigs[(int)_stats.TypeClass].health;
        }

       

        private void SetSortingOrderHeathBar()
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - transform.position.y * 10);
        }
    }
}