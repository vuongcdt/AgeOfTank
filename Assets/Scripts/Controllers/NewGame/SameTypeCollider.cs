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

        protected override void AwakeCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
            _actorName = _actorRun.name;
        }

        //OnTriggerEnter2D
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GetStartBarTag()))
            {
                MovetoOverStartBar();
                return;
            }

            if (!IsSameTag(other) || _actorRun.isAttack)
            {
                return;
            }

            var actorObstacle = other.GetComponentInParent<Actor>();
            if (_actorRun.id <= actorObstacle.id)
            {
                return;
            }

            _actorRun.ActorObstacle = actorObstacle;
            if (_actorRun.IsNearStartPoint())
            {
                return;
            }

            if (_actorRun.ActorsHead.Count > 2 && !_actorRun.IsNearStartPoint())
            {
                _actorRun.transform.DOKill();
                return;
            }

            MoveAcross();
        }

        private void MovetoOverStartBar()
        {
            if (_actorRun.ActorsHead.Count > 2 && !_actorRun.IsNearStartPoint())
            {
                _actorRun.transform.DOKill();
                return;
            }

            if (!_actorRun.ActorObstacle)
            {
                _actorRun.MoveToTarget();
                return;
            }

            MoveAcross();
        }

        private string GetStartBarTag()
        {
            return _actorRun.isPlayer ? CONSTANS.Tag.StartBarPlayer : CONSTANS.Tag.StartBarEnemy;
        }

        private void MoveAcross()
        {
            _actorRun.transform.DOKill();

            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posActor = _actorRun.transform.position;
            var durationMove = ActorConfig.durationMove * 0.1f;

            var posObstacle = _actorRun.ActorObstacle.transform.position;

            // var newPointTarget = _actorRun.IsNearStartPoint()
            //     ? posObstacle + offset + offsetCharacter
            //     : posActor + offset;  

            var newPointTarget = posObstacle + offset + offsetCharacter;

            _actorRun.transform
                .DOMove(newPointTarget, durationMove)
                .SetEase(Ease.Linear);
        }

        //OnTriggerStay2D
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsSameTag(other) || _actorRun.isAttack)
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

        //OnTriggerExit2D
        private void OnTriggerExit2D(Collider2D other)
        {
            if (_actorRun.isAttack || !_actorRun.ActorObstacle || !other.CompareTag(tag))
            {
                return;
            }

            var actorExit = other.GetComponentInParent<Actor>();

            if (!actorExit)
            {
                return;
            }

            if (_actorRun.ActorObstacle.id != actorExit.id)
            {
                return;
            }

            _actorRun.MoveToTarget();
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