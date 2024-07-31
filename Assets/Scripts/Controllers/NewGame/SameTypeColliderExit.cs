using Controllers.Game;
using UnityEngine;

namespace Controllers.NewGame
{
    public class SameTypeColliderExit : BaseGameController
    {
        private Actor _actorRun;

        protected override void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
        }

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
    }
}