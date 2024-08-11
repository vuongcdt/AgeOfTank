using System;
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

            // this.RegisterEvent<MoveHeadEvent>(e => MoveToCharacterAttack());
            this.RegisterEvent<MoveHeadEvent>(e => this.SendCommand(new MoveToCharacterAttackCommand(name)));

            _rg = GetComponent<Rigidbody2D>();
            MoveHead();
        }

        public void AttackCharacter(string keyBeaten)
        {
            _rg.mass = mass;
            _rg.velocity = Vector3.zero;

            this.SendCommand(new AttackCharacterCommand(keyBeaten, name));

            // if (!characterBeaten)
            // {
            //     _stats.IsAttack = false;
            //     return;
            // }
            //
            // _keyBeaten = characterBeaten.name;
            //
            // GamePlayModel.CharactersAttacking.TryAdd(name, _stats);
            // _stats.CharactersCanBeaten.TryAdd(_keyBeaten, characterBeaten);
            //
            // if (_stats.IsAttack)
            // {
            //     return;
            // }
            //
            // _stats.IsAttack = true;
            //
            // _attackCharacterIE = AttackCharacterIE();
            // StartCoroutine(_attackCharacterIE);
        }

        // private IEnumerator AttackCharacterIE()
        // {
        //     yield return new WaitForSeconds(CharacterConfig.attackTime);
        //     var isCharacterBeaten = GamePlayModel.Characters.ContainsKey(_keyBeaten);
        //
        //     if (!isCharacterBeaten)
        //     {
        //         _stats.CharactersCanBeaten.Remove(_keyBeaten);
        //         _keyBeaten = GetCharacterCanBeaten();
        //     }
        //     else
        //     {
        //         // this.SendCommand(new AttackCommand(keyBeaten, name));
        //         var statsBeaten = GamePlayModel.Characters[_keyBeaten];
        //         statsBeaten.Health.Value -= _stats.Damage;
        //     }
        //
        //     if (_keyBeaten is null)
        //     {
        //         _stats.IsAttack = false;
        //         StopCoroutine(_attackCharacterIE);
        //         MoveToCharacterAttack();
        //         yield break;
        //     }
        //
        //     _attackCharacterIE = AttackCharacterIE();
        //
        //     StartCoroutine(_attackCharacterIE);
        // }
        //
        // private string GetCharacterCanBeaten()
        // {
        //     string keyBeaten = null;
        //     foreach (var (_, character) in _stats.CharactersCanBeaten)
        //     {
        //         if (character.Stats.IsDeath)
        //         {
        //             continue;
        //         }
        //
        //         keyBeaten = character.name;
        //         break;
        //     }
        //
        //     return keyBeaten;
        // }

        public void MoveToCharacterAttack()
        {
            if (_stats.IsAttack)
            {
                return;
            }

            var characterAttackNearest = GetCharacterAttackNearest();
            if (characterAttackNearest)
            {
                var characterNearestPos = characterAttackNearest.transform.position;
                var newPointX = _stats.IsPlayer ? characterNearestPos.x - 0.5f : characterNearestPos.x + 0.5f;
                var newPoint = new Vector3(newPointX, characterNearestPos.y);

                var velocity = (newPoint - transform.position).normalized * CharacterConfig.speed;
                _rg.velocity = velocity;
            }
            else
            {
                MoveHead();
            }
        }

        private Character GetCharacterAttackNearest()
        {
            Character characterNearest = null;
            float minDistance = 10;
            foreach (var (_, characterStats) in GamePlayModel.CharactersAttacking)
            {
                if (_stats.Type == characterStats.Type || _stats.IsDeath || characterStats.IsDeath)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, characterStats.Transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    characterNearest = characterStats.Transform.GetComponent<Character>();
                }
            }

            return characterNearest;
        }

        public void MoveHead()
        {
            _rg.velocity = _stats.Target.normalized * CharacterConfig.speed;
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
            _stats.IsAttack = false;
            _stats.CharactersCanBeaten.Clear();
            GamePlayModel.CharactersAttacking.Remove(name);
            GamePlayModel.Characters.Remove(name);

            bool isHasCharacter = false;

            foreach (var (_, characterStats) in GamePlayModel.CharactersAttacking)
            {
                if (characterStats.Type == _stats.Type)
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