using System;
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
        public CharacterStats stats;

        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private bool isMoveTarget;

        public bool IsNearStartPoint()
        {
            var isNearStartPoint = Math.Abs(transform.position.x + stats.Source.x) >
                                   Mathf.Abs(stats.Source.x - stats.Target.x) - 0.8f;
            return isNearStartPoint;
        }

        public void RenderCharacter(string key)
        {
            stats = GamePlayModel.Characters[key];

            var idText = GetComponentInChildren<TextMesh>();
            avatar.sprite = ActorConfig.unitConfigs[(int)stats.TypeClass].imgAvatar;
            tag = stats.Tag;
            name = stats.Name;
            idText.text = stats.ID.ToString();
            idText.transform.localPosition = stats.IsPlayer ? new Vector3(-0.5f, 0.5f) : new Vector3(0.5f, 0.5f);

            gameObject.layer = stats.IsPlayer ? (int)ENUMS.Layer.Player : (int)ENUMS.Layer.Enemy;
            healthBar.SetActive(false);
            transform.position = stats.Source;
            transform.DOKill();
            avatar.flipX = !stats.IsPlayer;

            //Register BindableProperty
            stats.IsEnterSameTypeCollider.RegisterWithInitValue(MoveOverObstacle);
            stats.Health.Register(SetHealthBar);
            stats.CharacterBeaten.Register(Attack);

            MoveToTarget();
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

            healthSlider.value = newValue / ActorConfig.unitConfigs[(int)stats.TypeClass].health;
        }

        private void MoveOverObstacle(bool isEnterSameTypeCollider)
        {
            if (isEnterSameTypeCollider)
            {
                MoveAcross();
            }
            else
            {
                MoveToTarget();
            }
        }

        private void MoveAcross()
        {
            transform.DOKill();

            var offset = (stats.ID % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (stats.IsPlayer ? Vector3.right : Vector3.left) * 0.5f;
            // var posActor = transform.position;
            var durationMove = ActorConfig.durationMove * 0.1f;

            var posObstacle = stats.CharacterObstacle.Value.transform.position;

            // var newPointTarget = IsNearStartPoint()
            //     ? posObstacle + offset + offsetCharacter
            //     : posActor + offset;  

            var newPointTarget = posObstacle + offset + offsetCharacter;

            transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }

        private void Start()
        {
            this.RegisterEvent<ActorAttackPointEvent>(MoveToActorAttackX);
            this.RegisterEvent<MoveToTargetEvent>(MoveToTarget);
        }

        private void MoveToActorAttackX(ActorAttackPointEvent e)
        {
            if (stats.IsAttack || stats.Type == e.Type)
            {
                return;
            }

            if (stats.CharactersHead.Count >= 3 && !IsNearStartPoint())
            {
                return;
            }

            var warriorCollider = GetComponentInChildren<WarriorCollider>().CircleCollider;
            var newPosX = e.Position.x + (stats.IsPlayer ? -warriorCollider.radius * 2 + 0.05f : warriorCollider.radius * 2 - 0.05f);

            MoveToPoint(new Vector3(newPosX, transform.position.y));
        }

        
        private void Attack(Character characterBeaten)
        {
            transform.DOKill();
            GamePlayModel.CharactersAttacking.TryAdd(name, this);
            if (stats.IsAttack)
            {
                return;
            }
            this.SendCommand(new AttackCommand(characterBeaten,this));
            this.SendEvent(new ActorAttackPointEvent(transform.position, stats.Type));
        }

        private void MoveToPoint(Vector3 newPos)
        {
            transform.DOKill();
            if (!gameObject.activeSelf)
            {
                return;
            }

            var durationMoveToTarget = Vector3.Distance(transform.position, newPos) /
                Vector3.Distance(stats.Source, stats.Target) * ActorConfig.durationMove;

            transform
                .DOMove(newPos, durationMoveToTarget)
                .SetEase(Ease.Linear);
        }

        private void MoveToActorAttackNearest(Character actorAttackNearest)
        {
            var newPoint = GetActorAttackNearestPoint(actorAttackNearest);

            MoveToPoint(newPoint);
        }

        private void MoveToTarget(MoveToTargetEvent obj)
        {
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            var actorAttackNearest = ActorAttackNearest();
            if (!actorAttackNearest)
            {
                MoveToPoint(new Vector3(stats.Target.x, transform.position.y));
                return;
            }

            MoveToActorAttackNearestX(actorAttackNearest);
        }

        private void MoveToActorAttackNearestX(Character actorAttackNearest)
        {
            var newPoint = GetActorAttackNearestPoint(actorAttackNearest);

            MoveToPoint(new Vector3(newPoint.x, transform.position.y));
        }

        private Vector3 GetActorAttackNearestPoint(Character actorAttackNearest)
        {
            var children = GetComponentInChildren<WarriorCollider>();
            var warriorCollider = children.CircleCollider;
            var actorAttackNearestPosition = actorAttackNearest.transform.position;

            var random = (1 - Random.value) * 0.1f;

            var newPointX = actorAttackNearestPosition.x + (stats.Type == ENUMS.CharacterType.Player
                ? -warriorCollider.radius * 2 + random
                : warriorCollider.radius * 2 - random);

            return new Vector3(newPointX, actorAttackNearestPosition.y);
        }

        private Character ActorAttackNearest()
        {
            Character actorAttackNearest = null;
            var posActor = transform.position;
            float minDistance = 10;
            foreach (var pair in GamePlayModel.CharactersAttacking)
            {
                var actorAttack = pair.Value;
                var posActorAttack = actorAttack.transform.position;
                var distance = Vector3.Distance(posActor, posActorAttack);
                if (!actorAttack.gameObject.activeSelf)
                {
                    continue;
                }

                if (distance < minDistance && actorAttack.stats.Type != stats.Type)
                {
                    minDistance = distance;
                    actorAttackNearest = actorAttack;
                }
            }

            return actorAttackNearest;
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