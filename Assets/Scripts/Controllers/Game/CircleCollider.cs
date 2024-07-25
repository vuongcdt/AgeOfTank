using UnityEngine;

namespace Controllers.Game
{
    public class CircleCollider : BaseGameController
    {
        private Character _characterRun;
        private Character _characterObstacle;

        protected override void AwaitCustom()
        {
            _characterRun = GetComponentInParent<Character>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // if (!other.CompareTag(tag) || !tag.Contains(CONSTANTS.Tag.CircleCollider) ||
            //     !other.tag.Contains(CONSTANTS.Tag.CircleCollider))
            // {
            //     return;
            // }

            if (!other.CompareTag(tag))
            {
                return;
            }

            var characterObstacle = other.GetComponentInParent<Character>();

            if (_characterRun.Stats.ID <= characterObstacle.Stats.ID)
            {
                return;
            }

            _characterObstacle = characterObstacle;
            if (!_characterRun.IsMoveTarget)
            {
                return;
            }

            MoveOvercomeObstacles();
        }

        private void MoveOvercomeObstacles()
        {
            var offset = (_characterRun.Stats.ID % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (_characterRun.Stats.IsPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posObstacle = _characterObstacle.transform.position;
            var offsetNearTarget =
                Mathf.Abs(_characterRun.Stats.Target.x) - Mathf.Abs(_characterRun.transform.position.x) < 0.5f
                    ? offsetCharacter
                    : Vector3.zero;
            var newPoint = posObstacle + offset + offsetNearTarget;

            _characterRun.MoveNewPoint(newPoint);
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }

            var characterStay = other.GetComponentInParent<Character>();
            if (characterStay.Stats.ID > _characterRun.Stats.ID)
            {
                return;
            }

            _characterObstacle = characterStay;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(tag) || !tag.Contains(CONSTANTS.Tag.CircleCollider) ||
                !other.tag.Contains(CONSTANTS.Tag.CircleCollider))
            {
                return;
            }

            if (!_characterObstacle)
            {
                return;
            }

            var characterExit = other.GetComponentInParent<Character>();

            if (!characterExit)
            {
                return;
            }

            if (_characterObstacle.Stats.ID != characterExit.Stats.ID)
            {
                return;
            }

            if (!_characterRun.IsMoveTarget)
            {
                return;
            }

            _characterRun.MoveToTarget();
        }
    }
}