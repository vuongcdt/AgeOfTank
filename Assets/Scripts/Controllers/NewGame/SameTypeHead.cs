using Controllers.Game;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeHead : BaseGameController
    {
        private Actor _actorRun;
        private Actor _actorObstacle;
        private bool _isFullRow;

        protected override void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(CONSTANS.Tag.TopBar) || other.CompareTag(CONSTANS.Tag.BotBar))
            {
                CheckFullRow();
            }

            if (IsEnterObstacle(other))
            {
                return;
            }

            MoveNewPoint();
        }

        private void CheckFullRow()
        {
            _actorRun.transform.DOKill();
            _isFullRow = true;
            MoveFullRow();
        }

        private void OnTriggerStay2D(Collider2D other)
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

            _actorObstacle = actorStay;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsExitObstacle(other))
            {
                return;
            }

            // _actorObstacle = null;
            _actorRun.transform.DOKill();
            MoveToTarget();
            // _isFullRow = false;
        }

        private bool IsEnterObstacle(Collider2D other)
        {
            if (!IsSameTag(other))
            {
                return true;
            }

            _actorObstacle = other.GetComponentInParent<Actor>();

            if (_actorRun.id <= _actorObstacle.id)
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

        private void MoveNewPoint()
        {
            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 2f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posObstacle = _actorObstacle.transform.position;

            var newPointTarget = posObstacle + offset + offsetCharacter;

            var durationMove = ActorConfig.durationMove * 0.1f;
            
            _actorRun.transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }
        
        private void MoveFullRow()
        {
            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 2f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posObstacle = _actorObstacle.transform.position;

            var newPointTarget = posObstacle + (_isFullRow ? -offset - offsetCharacter : offset - offsetCharacter );

            var durationMove = _isFullRow
                ? ActorConfig.durationMove * 0.2f
                : ActorConfig.durationMove * 0.1f;
            
            _actorRun.transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }

        private bool IsSameTag(Collider2D other)
        {
            return other.CompareTag(tag)
                   && tag.Contains(CONSTANS.Tag.SameTypeCollider);
        }

        private bool IsExitObstacle(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return true;
            }

            if (!_actorObstacle)
            {
                return true;
            }

            var actorExit = other.GetComponentInParent<Actor>();

            if (!actorExit)
            {
                return true;
            }

            if (_actorObstacle.id != actorExit.id)
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