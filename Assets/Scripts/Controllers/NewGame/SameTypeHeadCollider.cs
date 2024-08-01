using Controllers.Game;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeHeadCollider : BaseGameController
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
            if (!other.CompareTag(CONSTANS.Tag.SameTypeCollider))
            {
                return;
            }

            var actorHead = other.GetComponentInParent<Actor>();
            if (actorHead.id == _actorRun.id)
            {
                return;
            }

            _actorRun.ActorsHead.TryAdd(actorHead.name, actorHead);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // if (!gameObject.activeSelf)
            // {
            //     this.SendEvent(new ActorAttackPointEvent(transform.position, _actorRun.type));
            //     return;
            // }

            if (!other.CompareTag(CONSTANS.Tag.SameTypeCollider))
            {
                return;
            }

            var sameTypeCollider = other.GetComponent<SameTypeCollider>();
            //
            // if (!sameTypeCollider)
            // {
            //     return;
            // }
            _actorRun.ActorsHead.Remove(sameTypeCollider.ActorName);
            // _actorRun.MoveToPoint(_actorRun.transform.position.x,sameTypeCollider.transform.position.x);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider2D>();
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(col.transform.position + (Vector3)col.offset, col.size);
        }
#endif
    }
}