using System;
using System.Collections;
using Commands.Game;
using Controllers.NewGame;
using DG.Tweening;
using Events;
using Interfaces;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using uPools;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        public bool IsMoveTarget => isMoveTarget;
        public CharacterStats Stats;

        [SerializeField] private int mass = 20;
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private bool isMoveTarget;

        private Rigidbody2D _rg;
        private Vector3 _targetMovePoint;

        public bool IsNearStartPoint()
        {
            var isNearStartPoint = Math.Abs(transform.position.x + Stats.Source.x) >
                                   Mathf.Abs(Stats.Source.x - Stats.Target.x) - 0.8f;
            return isNearStartPoint;
        }

        public void RenderCharacter(string key)
        {
            Stats = GamePlayModel.Characters[key];

            var idText = GetComponentInChildren<TextMesh>();
            avatar.sprite = ActorConfig.unitConfigs[(int)Stats.TypeClass].imgAvatar;
            tag = Stats.Tag;
            name = Stats.Name;
            idText.text = Stats.ID.ToString();
            idText.transform.localPosition = Stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            healthBar.SetActive(false);
            var random = (1 - Random.value) * 0.2f;
            transform.position = new Vector3(Stats.Source.x, Stats.Source.y + random);
            avatar.flipX = !Stats.IsPlayer;
            Stats.GameObject = gameObject;

            Stats.Health.Register(SetHealthBar);
            Stats.CharacterBeaten.Register(Attack);
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            this.RegisterEvent<CharacterAttackPointEvent>(e =>
            {
                if (e.Type == Stats.Type)
                {
                    return;
                }

                _targetMovePoint = e.Position;
            });
            _rg = GetComponent<Rigidbody2D>();
            _rg.velocity = Stats.Target.normalized * 0.2f;
            _targetMovePoint = Stats.Target;
        }

        private void SetHealthBar(float newValue)
        {
            if (newValue <= 0)
            {
                SetCharacterDeath();
                return;
            }

            healthBar.SetActive(true);
            SetSortingOrderHeathBar();

            healthSlider.value = newValue / ActorConfig.unitConfigs[(int)Stats.TypeClass].health;
        }

        private void Attack(Character characterBeaten)
        {
            if (!characterBeaten)
            {
                Stats.IsAttack = false;
                return;
            }

            _rg.mass = mass;
            _rg.velocity = Vector3.zero;

            GamePlayModel.CharactersAttacking.TryAdd(name, this);
            if (Stats.IsAttack)
            {
                return;
            }

            Stats.IsAttack = true;

            this.SendEvent(new CharacterAttackPointEvent(transform.position, Stats.Type));
        }

        private void SetCharacterDeath()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            transform.DOKill();
            GamePlayModel.CharactersAttacking.Remove(name);
            GamePlayModel.Characters.Remove(name);
            SharedGameObjectPool.Return(gameObject);
        }

        private void SetSortingOrderHeathBar()
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - transform.position.y * 10);
        }
    }
}