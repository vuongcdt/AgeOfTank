using System;
using System.Collections.Generic;
using Controllers.Game;
using DG.Tweening;
using Events;
using QFramework;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.NewGame
{
    public class Actor : BaseGameController
    {
        [SerializeField] private TextMesh idText;
        [SerializeField] private SpriteRenderer avatar;

        private readonly Dictionary<string, Actor> _actorsHead = new();
        private Actor _actorObstacle;

        public int id;
        public ENUMS.CharacterType type;
        public ENUMS.CharacterTypeClass typeClass;
        public bool isAttack;
        public bool isPlayer;
        public Vector3 start, end;

        public Dictionary<string, Actor> ActorsHead => _actorsHead;

        public Actor ActorObstacle
        {
            get => _actorObstacle;
            set => _actorObstacle = value;
        }

        protected override void AwaitCustom()
        {
            Init();
            this.RegisterEvent<ActorAttackPointEvent>(MoveToActorAttackX);
            this.RegisterEvent<MoveToTargetEvent>(e => MoveToTarget());
        }

        private void Init()
        {
            tag = type == ENUMS.CharacterType.Enemy
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;

            name = $"{type.ToString()}{id}";
            isPlayer = type == ENUMS.CharacterType.Player;
            idText.text = id.ToString();
            idText.transform.position = isPlayer ? new Vector3(-3, 0.5f) : new Vector3(3, 0.5f);
            avatar.flipX = !isPlayer;
        }

        private void MoveToActorAttackX(ActorAttackPointEvent e)
        {
            if (isAttack || type == e.Type)
            {
                return;
            }

            if (ActorsHead.Count >= 3 && !IsNearStartPoint())
            {
                return;
            }

            var warriorCollider = GetComponentInChildren<WarriorCollider>().CircleCollider;
            var newPosX = e.Position.x + (isPlayer ? -warriorCollider.radius * 2 : warriorCollider.radius * 2);

            MoveToPoint(new Vector3(newPosX, transform.position.y));
        }

        public bool IsNearStartPoint()
        {
            var isNearStartPoint = Math.Abs(transform.position.x + start.x) > Mathf.Abs(start.x - end.x) - 0.8f;
            return isNearStartPoint;
        }

        public void Attack()
        {
            transform.DOKill();
            isAttack = true;

            GamePlayModel.ActorsAttacking.TryAdd(name, this);

            this.SendEvent(new ActorAttackPointEvent(transform.position, type));
        }

        private void MoveToPoint(Vector3 newPos)
        {
            transform.DOKill();
            if (!gameObject.activeSelf)
            {
                return;
            }

            var durationMoveToTarget = Vector3.Distance(transform.position, newPos) /
                Vector3.Distance(start, end) * ActorConfig.durationMove;

            transform
                .DOMove(newPos, durationMoveToTarget)
                .SetEase(Ease.Linear);
        }

        public void MoveToActorAttackNearest(Actor actorAttackNearest)
        {
            var newPoint = GetActorAttackNearestPoint(actorAttackNearest);

            MoveToPoint(newPoint);
        }

        public void MoveToTarget()
        {
            var actorAttackNearest = ActorAttackNearest();
            if (!actorAttackNearest)
            {
                MoveToPoint(new Vector3(end.x, transform.position.y));
                return;
            }

            MoveToActorAttackNearestX(actorAttackNearest);
        }

        private void MoveToActorAttackNearestX(Actor actorAttackNearest)
        {
            var newPoint = GetActorAttackNearestPoint(actorAttackNearest);

            MoveToPoint(new Vector3(newPoint.x, transform.position.y));
        }

        private Vector3 GetActorAttackNearestPoint(Actor actorAttackNearest)
        {
            var warriorCollider = GetComponentInChildren<WarriorCollider>().CircleCollider;
            Vector3 actorAttackNearestPosition = actorAttackNearest.transform.position;
            var random = (1 - Random.value) * 0.1f;

            var newPointX = actorAttackNearestPosition.x + (type == ENUMS.CharacterType.Player
                ? -warriorCollider.radius * 2 + random
                : warriorCollider.radius * 2 - random);

            return new Vector3(newPointX, actorAttackNearestPosition.y);
        }

        private Actor ActorAttackNearest()
        {
            Actor actorAttackNearest = null;
            var posActor = transform.position;
            float minDistance = 10;
            foreach (var pair in GamePlayModel.ActorsAttacking)
            {
                var actorAttack = pair.Value;
                var posActorAttack = actorAttack.transform.position;
                var distance = Vector3.Distance(posActor, posActorAttack);
                if (!actorAttack.gameObject.activeSelf)
                {
                    continue;
                }

                if (distance < minDistance && actorAttack.type != type)
                {
                    minDistance = distance;
                    actorAttackNearest = actorAttack;
                }
            }

            return actorAttackNearest;
        }
    }
}