using Controllers.Game;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeColliderExit : BaseGameController
    {
        private Actor _actorRun;

        protected override void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
        }

        private void OnTriggerExit2D(Collider2D other) //OnTriggerExit2D
        {
            if (IsNotObstacleExit(other))
            {
                return;
            }

            // _actorObstacle = null;
            _actorRun.transform.DOKill();
            MoveToTarget();
        }


        private bool IsNotObstacleExit(Collider2D other)
        {
            if (_actorRun.isAttack)
            {
                return true;
            }

            if (!_actorRun.ActorObstacle)
            {
                return true;
            }

            if (!other.CompareTag(tag))
            {
                return true;
            }

            var actorExit = other.GetComponentInParent<Actor>();

            if (!actorExit)
            {
                return true;
            }

            if (_actorRun.ActorObstacle.id != actorExit.id)
            {
                return true;
            }

            // if (!_actorRun.IsMoveTarget)
            // {
            //     return;
            // }

            return false;
        }

        private void MoveToTarget()
        {
            var posRun = _actorRun.transform.position;
            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                posRun.x,
                _actorRun.start.x,
                _actorRun.end.x,
                ActorConfig.durationMove);

            _actorRun.transform
                .DOMove(new Vector3(_actorRun.end.x, posRun.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}