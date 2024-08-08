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
        [SerializeField] private CharacterStats _stats;
        private Character _characterBeaten;
        private IEnumerator _moveToPoint;

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

            var idText = GetComponentInChildren<TextMesh>();
            avatar.sprite = CharacterConfig.unitConfigs[(int)_stats.TypeClass].imgAvatar;
            tag = _stats.Tag;
            name = _stats.Name;
            idText.text = _stats.ID.ToString();
            idText.transform.localPosition = _stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            healthBar.SetActive(false);
            var random = (1 - Random.value) * 0.2f;
            transform.position = new Vector3(_stats.Source.x, _stats.Source.y + random);
            avatar.flipX = !_stats.IsPlayer;
            _stats.GameObject = gameObject;
            Init();
        }

        private void Init()
        {
            _stats.Health.Register(SetHealthBar);

            this.RegisterEvent<MoveHeadEvent>(e => { MoveHead(); });

            _rg = GetComponent<Rigidbody2D>();
            _rg.velocity = _stats.Target.normalized * CharacterConfig.speed;
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

            // this.SendCommand(new AttackCommand(characterBeaten, this));
            this.SendCommand(new AttackCommand(characterBeaten.name, name));
        }

        public void MoveToPoint()
        {
            if (_moveToPoint != null)
            {
                StopCoroutine(_moveToPoint);
            }

            StartCoroutine(MoveToPointIE(_stats.Target));
        }

        private Character GetCharacterAttackNearest()
        {
            Character characterNearest = null;
            float minDistance = 10;
            foreach (var (key, character) in GamePlayModel.CharactersAttacking)
            {
                if (_stats.Type == character.Stats.Type || _stats.IsDeath || character.Stats.IsDeath)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    characterNearest = character;
                }
            }

            return characterNearest;
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

            if (_moveToPoint != null)
            {
                StopCoroutine(_moveToPoint);
            }

            StartCoroutine(MoveToPointIE(_stats.Target));
        }

        private IEnumerator MoveToPointIE(Vector3 point)
        {
            var magnitude = _rg.velocity.magnitude;
            var distance = Vector3.Distance(point, transform.position);
            if (distance < 0.1f || _isAttack)
            {
                yield break;
            }

            if (magnitude < 0.2f && !_isAttack)
            {
                // _rg.AddForce((point - transform.position).normalized * speed);
                _rg.velocity = (point - transform.position).normalized * CharacterConfig.speed;
            }

            yield return new WaitForSeconds(0.2f);
            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPoint =
                    new Vector3(_stats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f,
                        characterNearestPos.y);


                if (_moveToPoint != null)
                {
                    StopCoroutine(_moveToPoint);
                }

                _moveToPoint = MoveToPointIE(newPoint);
                StartCoroutine(_moveToPoint);
                yield break;
            }

            StartCoroutine(MoveToPointIE(point));
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

            healthSlider.value = newValue / CharacterConfig.unitConfigs[(int)_stats.TypeClass].health;
        }

        private void SetCharacterDeath()
        {
            _isAttack = false;
            _stats.CharactersCanBeaten.Clear();
            GamePlayModel.CharactersAttacking.Remove(name);
            GamePlayModel.Characters.Remove(name);

            bool isHasCharacter = false;
            _rg.mass = 1;

            foreach (var (_, character) in GamePlayModel.CharactersAttacking)
            {
                if (character._stats.Type == _stats.Type)
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