﻿using System;
using Commands.Game;
using Interfaces;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        public CharacterStats Stats => _stats;

        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private GameObject hunterCollider;

        private Rigidbody2D _rg;
        private CharacterStats _stats;
        private string _keyBeaten;
        private Animator _animator;

        private static readonly int WalkAnimator = Animator.StringToHash("walk");
        private static readonly int SpeedAnimator = Animator.StringToHash("speed");

        public bool IsNearStartPoint()
        {
            var isNearStartPoint = Math.Abs(transform.position.x + Stats.Source.x) >
                                   Mathf.Abs(Stats.Source.x - Stats.Target.x) - 0.8f;
            return isNearStartPoint;
        }

        public void RenderCharacter(string key)
        {
            _stats = GamePlayModel.Characters[key];
            name = _stats.Name;

            SetTagAndLayer();
            SetAvatar();
            SetHealthBar();
            SetPosition();
            Init();
        }

        private void SetPosition()
        {
            var random = (1 - Random.value) * 0.2f;
            var transformCache = transform;

            transformCache.position = new Vector3(_stats.Source.x, _stats.Source.y + random);
            _stats.Transform = transformCache;
        }

        private void SetTagAndLayer()
        {
            tag = _stats.Tag;
            var warriorCollision = GetComponentInChildren<WarriorCollision>();
            warriorCollision.SetTagAndLayer(_stats.Type);

            var isHunterClass = (int)_stats.TypeClass % 3 == 1;
            hunterCollider.SetActive(isHunterClass);
            gameObject.layer = isHunterClass ? (int)ENUMS.Layer.SameTypeHunter : (int)ENUMS.Layer.SameType;
        }

        private void SetHealthBar()
        {
            var idText = GetComponentInChildren<TextMesh>();
            idText.text = _stats.ID.ToString();
            idText.transform.localPosition = _stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            healthBar.SetActive(false);
        }

        private void SetAvatar()
        {
            GetComponent<SortingGroup>().sortingOrder = GetSortingOrder();
            
            var avatarPrefab = CharacterConfig.unitConfigs[(int)_stats.TypeClass].prefabAvatar;
            Instantiate(avatarPrefab, transform);

            _animator = GetComponentInChildren<Animator>();
            _animator.transform.rotation = new Quaternion(0, _stats.IsPlayer ? 180 : 0, 0, 0);
        }

        private void Init()
        {
            _stats.Health.Register(SetHealthBar);

            this.RegisterEvent<MoveToCharacterAttack>(e => this.SendCommand(new MoveToCharacterAttackCommand(name)));

            _rg = GetComponent<Rigidbody2D>();
            MoveHead(CharacterConfig.speed);
        }

        public void MoveHead(float speed)
        {
            _rg.velocity = _stats.Target.normalized * speed;
            _animator.SetTrigger(WalkAnimator);
        }

        private void SetHealthBar(float newValue)
        {
            if (newValue <= 0)
            {
                this.SendCommand(new SetCharacterDeathCommand(name));
                return;
            }

            healthBar.SetActive(true);
            healthBar.GetComponent<Canvas>().sortingOrder = GetSortingOrder();

            healthSlider.value = newValue / CharacterConfig.unitConfigs[(int)_stats.TypeClass].health;
        }


        private int GetSortingOrder()
        {
            return Mathf.CeilToInt(10 - transform.position.y * 10);
        }
    }
}