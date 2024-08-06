using System;
using System.Collections;
using Commands.Game;
using Interfaces;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using uPools;
using Random = UnityEngine.Random;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        public bool IsMoveTarget => isMoveTarget;
        public CharacterStats Stats => _stats;

        public bool IsAttack
        {
            get => _isAttack;
            set => _isAttack = value;
        }

        [SerializeField] private int mass = 20;
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private bool isMoveTarget;

        private Rigidbody2D _rg;
        private bool _isAttack;
        private CharacterStats _stats;
        private Character _characterBeaten;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _stats.Health.Register(SetHealthBar);

            this.RegisterEvent<MoveHeadEvent>(e => { MoveHead(); });

            _rg = GetComponent<Rigidbody2D>();
            _rg.velocity = _stats.Target.normalized * 0.2f;
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

            var idText = GetComponentInChildren<TextMesh>();
            avatar.sprite = ActorConfig.unitConfigs[(int)_stats.TypeClass].imgAvatar;
            tag = _stats.Tag;
            name = _stats.Name;
            idText.text = _stats.ID.ToString();
            idText.transform.localPosition = _stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            healthBar.SetActive(false);
            var random = (1 - Random.value) * 0.2f;
            transform.position = new Vector3(_stats.Source.x, _stats.Source.y + random);
            avatar.flipX = !_stats.IsPlayer;
            _stats.GameObject = gameObject;
        }

        public void Attack(Character characterBeaten)
        {
            _rg.mass = mass;
            _rg.velocity = Vector3.zero;

            if (!characterBeaten)
            {
                _isAttack = false;
                return;
            }

            _characterBeaten = characterBeaten;

            GamePlayModel.CharactersAttacking.TryAdd(name, this);
            _stats.CharactersCanBeaten.TryAdd(_characterBeaten.name, _characterBeaten);

            if (_isAttack)
            {
                return;
            }

            _isAttack = true;

            this.SendCommand(new AttackCommand(characterBeaten, this));
        }

        public void MoveToPoint(Vector3 point)
        {
            return;
            var position = transform.position;
            var newPoint = new Vector3(position.x, point.y) - position;
            
            Debug.Log($"_character {name} {position} point {point} newPoint {newPoint} {newPoint.normalized}");

            StartCoroutine(AddVelocityIE(newPoint.normalized * 0.2f));
        }

        public void MoveHead()
        {
            if (_characterBeaten && _characterBeaten._stats.IsDeath)
            {
                _isAttack = false;
            }

            if (_isAttack || _stats.IsDeath)
            {
                return;
            }

            StartCoroutine(AddVelocityIE(_stats.Target.normalized * 0.2f));
        }

        private IEnumerator AddVelocityIE(Vector3 velocity)
        {
            var magnitude = _rg.velocity.magnitude;

            if (magnitude < 0.2f && !_isAttack)
            {
                _rg.velocity = velocity;
            }

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(AddVelocityIE(velocity));
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

            healthSlider.value = newValue / ActorConfig.unitConfigs[(int)_stats.TypeClass].health;
        }

        private void SetCharacterDeath()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            _isAttack = false;
            GamePlayModel.CharactersAttacking.Remove(name);
            GamePlayModel.Characters.Remove(name);
            bool isHasCharacter = false;
            _rg.mass = 1;

            foreach (var pair in GamePlayModel.CharactersAttacking)
            {
                if (pair.Value._stats.Type == _stats.Type)
                {
                    isHasCharacter = true;
                }
            }

            if (!isHasCharacter)
            {
                this.SendEvent<MoveHeadEvent>();
            }

            SharedGameObjectPool.Return(gameObject);
        }

        private void SetSortingOrderHeathBar()
        {
            healthBar.GetComponent<Canvas>().sortingOrder =
                Mathf.CeilToInt(10 - transform.position.y * 10);
        }
    }
}