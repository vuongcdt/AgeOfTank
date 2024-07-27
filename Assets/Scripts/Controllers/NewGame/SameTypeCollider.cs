using Controllers.Game;
using DG.Tweening;
using QFramework;
using Systems;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class SameTypeCollider : BaseGameController
    {
        private CharacterConfig _actorConfig;
        private Actor _actorRun;
        private Actor _actorObstacle;

        protected override async void AwaitCustom()
        {
            _actorRun = GetComponentInParent<Actor>();
            _actorConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(CONSTANS.Tag.TopBar))
            {
                Debug.Log("TOP BAR");
            }

            if (other.CompareTag(CONSTANS.Tag.BotBar))
            {
                Debug.Log("BOT BAR");
            }

            if (!IsSameTag(other))
            {
                return;
            }

            _actorObstacle = other.GetComponentInParent<Actor>();

            if (_actorRun.id <= _actorObstacle.id)
            {
                return;
            }

            // if (!_actorRun.IsMoveTarget)
            // {
            //     return;
            // }

            if (_actorRun.isAttack)
            {
                return;
            }

            MoveNewPoint();
        }

        private void MoveNewPoint()
        {
            var offset = (_actorRun.id % 2 == 0 ? Vector3.up : Vector3.down) * 2f;
            var offsetCharacter = (_actorRun.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posObstacle = _actorObstacle.transform.position;

            var newPointTarget = posObstacle + offset + offsetCharacter;

            _actorRun.transform
                .DOMove(newPointTarget, _actorConfig.durationMove * 0.1f)
                .SetEase(Ease.Linear);
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsSameTag(other)) return;

            var actorStay = other.GetComponentInParent<Actor>();
            if (actorStay.id > _actorRun.id)
            {
                return;
            }

            _actorObstacle = actorStay;
        }

        private bool IsSameTag(Collider2D other)
        {
            return other.CompareTag(tag)
                   && tag.Contains(CONSTANS.Tag.SameTypeCollider);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }

            if (!_actorObstacle)
            {
                return;
            }

            var actorExit = other.GetComponentInParent<Actor>();

            if (!actorExit)
            {
                return;
            }

            if (_actorObstacle.id != actorExit.id)
            {
                return;
            }

            // if (!_actorRun.IsMoveTarget)
            // {
            //     return;
            // }

            if (_actorRun.isAttack)
            {
                return;
            }

            _actorRun.transform.DOKill();
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            var posRun = _actorRun.transform.position;
            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                posRun.x,
                _actorRun.start.x,
                _actorRun.end.x,
                _actorConfig.durationMove);

            _actorRun.transform
                .DOMove(new Vector3(_actorRun.end.x, posRun.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}