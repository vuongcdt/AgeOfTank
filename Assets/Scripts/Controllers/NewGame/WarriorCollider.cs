using Controllers.Game;
using DG.Tweening;
using Events;
using QFramework;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers.NewGame
{
    public class WarriorCollider : BaseGameController
    {
        private Actor _actor;
        private Actor _actorBeaten;
        private CircleCollider2D _circleCollider;

        public CircleCollider2D CircleCollider => _circleCollider;

        protected override void AwakeCustom()
        {
            Init();
        }

        private void Init()
        {
            _circleCollider = GetComponent<CircleCollider2D>();
            _actor = GetComponentInParent<Actor>();
            tag = _actor.type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderPlayer
                : CONSTANS.Tag.WarriorColliderEnemy;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(_actor.type == ENUMS.CharacterType.Player
                    ? CONSTANS.Tag.WarriorColliderEnemy
                    : CONSTANS.Tag.WarriorColliderPlayer))
            {
                _actorBeaten = other.GetComponentInParent<Actor>();
                _actor.Attack();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var isPassTag = other.CompareTag(_actor.type == ENUMS.CharacterType.Player
                ? CONSTANS.Tag.WarriorColliderEnemy
                : CONSTANS.Tag.WarriorColliderPlayer);

            if (!isPassTag)
            {
                return;
            }

            var actorExit = other.GetComponentInParent<Actor>();

            if (!_actorBeaten && !actorExit && _actorBeaten.id != actorExit.id)
            {
                return;
            }

            // _actor.MoveToTarget();
            MoveToCompetitor();
        }

        private void MoveToCompetitor()
        {
            if (!_actor.gameObject.activeSelf)
            {
                return;
            }

            var actorAttackNearest = ActorAttackNearest();

            if (!actorAttackNearest)
            {
                // _actor.MoveToTarget();
                this.SendEvent<MoveToTargetEvent>();
                return;
            }

            Vector3 actorAttackNearestPosition = actorAttackNearest.transform.position;

            var random = Random.value * 0.1f;
            var durationMove = 6f;

            var posActorX = actorAttackNearestPosition.x + (_actor.type == ENUMS.CharacterType.Player
                ? -_circleCollider.radius * 2 + random
                : _circleCollider.radius * 2 - random);

            _actor.transform
                .DOMove(new Vector3(posActorX, actorAttackNearestPosition.y), durationMove)
                .OnComplete(MoveToCompetitor);
        }

        private Actor ActorAttackNearest()
        {
            Actor actorAttackMin = null;
            var posActor = _actor.transform.position;
            float minDistance = 10;
            // foreach (var pair in GamePlayModel.ActorsAttacking)
            // {
            //     var actorAttack = pair.Value;
            //     var posActorAttack = actorAttack.transform.position;
            //     var distance = Vector3.Distance(posActor, posActorAttack);
            //     if (!actorAttack.gameObject.activeSelf)
            //     {
            //         continue;
            //     }
            //
            //     // if (distance < minDistance && actorAttack.type != _actor.type)
            //     // {
            //     //     minDistance = distance;
            //     //     // point = posActorAttack;
            //     //     actorAttackMin = actorAttack;
            //     // }
            // }

            return actorAttackMin;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<CircleCollider2D>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
#endif
    }
}