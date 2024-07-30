using System;
using System.Collections.Generic;
using Controllers.Game;
using DG.Tweening;
using Events;
using QFramework;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class Actor : BaseGameController
    {
        [SerializeField] private TextMesh idText;
        [SerializeField] private SpriteRenderer avatar;

        private readonly Dictionary<string, Actor> _actorsHead = new();
        private bool _isBehind;

        public int id;
        public ENUMS.CharacterType type;
        public ENUMS.CharacterTypeClass typeClass;
        public bool isAttack;
        public bool isPlayer;
        public Vector3 start, end;
        public Dictionary<string, Actor> ActorsHead => _actorsHead;

        public bool IsBehind
        {
            get => _isBehind;
            set => _isBehind = value;
        }

        protected override void AwaitCustom()
        {
            Init();
            this.RegisterEvent<ActorAttackPointEvent>(MoveToActorAttack);
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

        private void MoveToActorAttack(ActorAttackPointEvent e)
        {
            if (isAttack)
            {
                return;
            }

            var circleCollider = GetComponentInChildren<WarriorCollider>().CircleCollider;
            var posX = e.Pos.x + (isPlayer ? -circleCollider.radius : circleCollider.radius);

            
            foreach (var (key, value) in ActorsHead)
            {
                // Debug.Log($"{name} {key} {value}");
            }
            
            if (ActorsHead.Count >= 3 || isAttack || !_isBehind)
            {
                return;
            }
            
            // if (e.Type != type && ActorsHead.Count < 3)
            // {
            //     MoveToPoint(transform.position.x, posX);
            //     return;
            // }
            //
            // if (ActorsHead.Count >= 3 || isAttack || !_isBehind)
            // {
            //     return;
            // }

            _isBehind = false;
            MoveToPoint(transform.position.x, posX);
        }

        public void Attack()
        {
            transform.DOKill();
            isAttack = true;

            GamePlayModel.ActorsAttacking.TryAdd(name, this);
            this.SendEvent(new ActorAttackPointEvent(transform.position, type));
        }

        private void MoveToTarget()
        {
            MoveToPoint(transform.position.x, end.x);
        }

        public void MoveToPoint(float currentX, float newX)
        {
            transform.DOKill();
            if (!gameObject.activeSelf)
            {
                return;
            }

            var durationMoveToTarget = Utils.GetDurationMoveToPoint(
                currentX,
                newX,
                start.x,
                end.x,
                ActorConfig.durationMove);

            transform
                .DOMove(new Vector3(newX, transform.position.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}