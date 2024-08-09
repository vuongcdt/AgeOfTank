using System;
using System.Collections;
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

        [SerializeField] private int mass = 20;
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;

        private Rigidbody2D _rg;
        private bool _isAttack;
        private CharacterStats _stats;
        private Character _characterBeaten;
        private IEnumerator _moveToPointIE;
        private IEnumerator _attackCharacterIE;

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

            this.RegisterEvent<MoveHeadEvent>(e => MoveToCharacterAttack());

            _rg = GetComponent<Rigidbody2D>();
            MoveHead();
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

            _attackCharacterIE = AttackCharacterIE(characterBeaten.name);
            StartCoroutine(_attackCharacterIE);
        }

        private IEnumerator AttackCharacterIE(string keyBeaten)
        {
            yield return new WaitForSeconds(CharacterConfig.attackTime);
            var isCharacterBeaten = GamePlayModel.Characters.ContainsKey(keyBeaten);

            if (!isCharacterBeaten)
            {
                _stats.CharactersCanBeaten.Remove(keyBeaten);
                keyBeaten = GetCharacterCanBeaten();
            }
            else
            {
                // this.SendCommand(new AttackCommand(keyBeaten, name));
                var statsBeaten = GamePlayModel.Characters[keyBeaten];
                statsBeaten.Health.Value -= _stats.Damage;
            }

            if (keyBeaten is null)
            {
                _isAttack = false;
                StopCoroutine(_attackCharacterIE);
                //moveToCharacterAttack
                MoveToCharacterAttack();
                yield break;
            }

            _attackCharacterIE = AttackCharacterIE(keyBeaten);

            StartCoroutine(_attackCharacterIE);
        }

        public void MoveToCharacterAttack()
        {
            if (_isAttack)
            {
                return;
            }

            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPointX = _stats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f;
                var newPoint = new Vector3(newPointX, characterNearestPos.y);

                var velocity = (newPoint - transform.position).normalized;
                // Debug.Log(
                //     $"{name} {transform.position} {characterAttackNearest.name} {characterNearestPos} {velocity}");
                _rg.velocity = velocity;
            }
            else
            {
                MoveHead();
            }
        }

        private string GetCharacterCanBeaten()
        {
            string keyBeaten = null;
            foreach (var (_, character) in _stats.CharactersCanBeaten)
            {
                if (character.Stats.IsDeath)
                {
                    continue;
                }

                keyBeaten = character.name;
                break;
            }

            return keyBeaten;
        }

        private Character GetCharacterAttackNearest()
        {
            Character characterNearest = null;
            float minDistance = 10;
            foreach (var (_, character) in GamePlayModel.CharactersAttacking)
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
            _rg.velocity = _stats.Target.normalized * CharacterConfig.speed;
        }

        public void AddForce()
        {
            _rg.AddForce(_stats.Target.normalized * CharacterConfig.speed);
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
            _rg.mass = 1;
            _isAttack = false;
            _stats.CharactersCanBeaten.Clear();
            GamePlayModel.CharactersAttacking.Remove(name);
            GamePlayModel.Characters.Remove(name);

            bool isHasCharacter = false;

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
                // this.SendEvent(new InitCharacter(_stats.TypeClass));
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