using Controllers.Game;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeCollider : BaseGameController
    {
        private Actor _actorRun;
        private Actor _actorObstacle;
        private bool _isFullRow;
        private BoxCollider2D _boxCollider2D;
        private string _actorName;

        public string ActorName => _actorName;

        protected override void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _actorName = _actorRun.name;
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

            var actorsHeadCount = _actorRun.ActorsHead.Count;
            if (actorsHeadCount > 2)
            {
                _actorRun.transform.DOKill();
                _actorRun.IsBehind = true;
                return;
            }

            MoveNewPoint();
        }

        private void CheckFullRow()
        {
            _isFullRow = true;
            _actorRun.transform.position += _actorRun.isPlayer
                ? -new Vector3(_boxCollider2D.size.x / 2, 0)
                : new Vector3(_boxCollider2D.size.x / 2, 0);
            MoveNewPoint();
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
            _isFullRow = false;
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
            _actorRun.transform.DOKill();

            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posActor = _actorRun.transform.position;
            var durationMove = ActorConfig.durationMove * 0.2f;

            var newPointTarget = _isFullRow
                ? GetPointMoveFullRow(posActor, offset, offsetCharacter)
                : GetPointMoveOvercomeObstacle(posActor, offset, offsetCharacter);


            _actorRun.transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }

        private Vector3 GetPointMoveFullRow(Vector3 posActor, Vector3 offset, Vector3 offsetCharacter)
        {
            return posActor + (_isFullRow ? -offset - offsetCharacter : offset - offsetCharacter);
        }

        private Vector3 GetPointMoveOvercomeObstacle(Vector3 posActor, Vector3 offset, Vector3 offsetCharacter)
        {
            return posActor + offset;
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