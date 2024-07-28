using Controllers.Game;
using DG.Tweening;
using Interfaces;
using QFramework;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class Actor : BaseGameController
    {
        [SerializeField] private TextMesh idText;
        [SerializeField] private SpriteRenderer avatar;
        
        public int id;
        public ENUMS.CharacterType type;
        public ENUMS.CharacterTypeClass typeClass;
        public bool isAttack;
        public bool isPlayer;
        public Vector3 start, end;

        protected override void AwaitCustom()
        {
            tag = type == ENUMS.CharacterType.Enemy
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;
            name = $"{type.ToString()}{id}";
            isPlayer = type == ENUMS.CharacterType.Player;
            idText.text = id.ToString();
            avatar.flipX = !isPlayer;
        }

        public void Attack()
        {
            transform.DOKill();
            isAttack = true;
            var actorsAttacking = this.GetModel<IGamePlayModel>().ActorsAttacking;
            actorsAttacking.TryAdd(name,this);
            // if (!actorsAttacking.TryAdd())
            // {
            //     actorsAttacking.TryAdd(name,this);
            // }
        }

        public void MoveToPoint(Vector3 pos, float time)
        {
            transform.DOMove(pos, time);
        }

        public void MoveToTarget()
        {
            var posRun = transform.position;
            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                posRun.x,
                start.x,
                end.x,
                ActorConfig.durationMove);

            transform
                .DOMove(new Vector3(end.x, posRun.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}