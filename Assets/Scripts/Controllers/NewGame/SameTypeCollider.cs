using System;
using Controllers.Game;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeCollider : BaseGameController
    {
        private Actor _actorRun;

        private string _actorName;

        public string ActorName => _actorName;

        protected override void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
            _actorName = _actorRun.name;
        }

        private void OnTriggerEnter2D(Collider2D other) //OnTriggerEnter2D
        {
            if (IsNotEnterObstacle(other))
            {
                return;
            }

            if (_actorRun.ActorsHead.Count > 2)
            {
                _actorRun.transform.DOKill();
                return;
            }

            MoveAcross();
        }

        private bool IsNotEnterObstacle(Collider2D other)
        {
            if (!IsSameTag(other))
            {
                return true;
            }

            _actorRun.ActorObstacle = other.GetComponentInParent<Actor>();

            if (_actorRun.id <= _actorRun.ActorObstacle.id)
            {
                return true;
            }

            // if (!_actorRun.IsMoveTarget)
            // {
            //     return;
            // }

            if (_actorRun.isAttack)
            {
                return true;
            }


            return false;
        }

        private void MoveAcross()
        {
            _actorRun.transform.DOKill();

            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posActor = _actorRun.transform.position;
            var durationMove = ActorConfig.durationMove * 0.1f;

            var posObstacle = _actorRun.ActorObstacle.transform.position;
            bool isNearStartPoint = Math.Abs(posActor.x + _actorRun.start.x) > 4;

            var newPointTarget = isNearStartPoint ? posObstacle + offset + offsetCharacter : posActor + offset;

            _actorRun.transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerStay2D(Collider2D other) //OnTriggerStay2D
        {
            if (!IsSameTag(other))
            {
                return;
            }

            var actorStay = other.GetComponentInParent<Actor>();
            if (actorStay.id > _actorRun.id)
            {
                return;
            }

            _actorRun.ActorObstacle = actorStay;
        }

        private bool IsSameTag(Collider2D other)
        {
            return other.CompareTag(tag)
                   && tag.Contains(CONSTANS.Tag.SameTypeCollider);
        }

        // private void OnTriggerExit2D(Collider2D other) //OnTriggerExit2D
        // {
        //     if (IsNotObstacleExit(other))
        //     {
        //         return;
        //     }
        //
        //     // _actorObstacle = null;
        //     _actorRun.transform.DOKill();
        //     MoveToTarget();
        // }
        //
        //
        // private bool IsNotObstacleExit(Collider2D other)
        // {
        //     if (!other.CompareTag(tag))
        //     {
        //         return true;
        //     }
        //
        //     if (!_actorRun.ActorObstacle)
        //     {
        //         return true;
        //     }
        //
        //     var actorExit = other.GetComponentInParent<Actor>();
        //
        //     if (!actorExit)
        //     {
        //         return true;
        //     }
        //
        //     if (_actorRun.ActorObstacle.id != actorExit.id)
        //     {
        //         return true;
        //     }
        //
        //     // if (!_actorRun.IsMoveTarget)
        //     // {
        //     //     return;
        //     // }
        //
        //     if (_actorRun.isAttack)
        //     {
        //         return true;
        //     }
        //
        //     Debug.Log($"{_actorRun.name} EXIT {_actorRun.ActorObstacle.name}");
        //
        //     return false;
        // }
        //
        // private void MoveToTarget()
        // {
        //     var posRun = _actorRun.transform.position;
        //     var durationMoveToTarget = Utils.GetDurationMoveToTarget(
        //         posRun.x,
        //         _actorRun.start.x,
        //         _actorRun.end.x,
        //         ActorConfig.durationMove);
        //
        //     _actorRun.transform
        //         .DOMove(new Vector3(_actorRun.end.x, posRun.y), durationMoveToTarget)
        //         .SetEase(Ease.Linear);
        // }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider2D>();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
#endif
    }
}