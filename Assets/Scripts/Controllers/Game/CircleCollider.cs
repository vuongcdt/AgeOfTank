using QFramework;
using Systems;
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
            if (IsCircleColliderTag(other)) return;

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

        // private async void MoveOvercomeObstacles()
        // {
        //     var offset = (_characterRun.Stats.ID % 2 == 0 ? Vector3.up : Vector3.down) * 1.25f;
        //     var offsetCharacter = (_characterRun.Stats.IsPlayer ? Vector3.right : Vector3.left) * 0.7f;
        //     var posObstacle = _characterObstacle.transform.position;
        //     var offsetNearTarget =
        //         Mathf.Abs(_characterRun.Stats.Target.x - _characterRun.transform.position.x) < 0.5f
        //             ? Vector3.zero
        //             : offsetCharacter;
        //     var newPoint = posObstacle + offset + offsetNearTarget;
        //     var characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        //
        //     _characterRun.MoveNewPoint(newPoint, characterConfig.durationMove * 0.15f);
        // }

        private async void MoveOvercomeObstacles()
        {
            var characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        
            var offset = (_characterRun.Stats.ID % 2 == 0 ? Vector3.up : Vector3.down) * 1.5f;
            var offsetCharacter = (_characterRun.Stats.IsPlayer ? Vector3.right : Vector3.left) * 0.75f;
            var posObstacle = _characterObstacle.transform.position;
        
            var newPointTarget = posObstacle + offset + offsetCharacter;
          
            _characterRun.MoveNewPoint(newPointTarget, characterConfig.durationMove * 0.15f);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsCircleColliderTag(other)) return;

            var characterStay = other.GetComponentInParent<Character>();
            if (characterStay.Stats.ID > _characterRun.Stats.ID)
            {
                return;
            }

            _characterObstacle = characterStay;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsCircleColliderTag(other)) return;

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

        private bool IsCircleColliderTag(Collider2D other)
        {
            return !other.CompareTag(tag) || !tag.Contains(CONSTANTS.Tag.CircleCollider) ||
                   !other.tag.Contains(CONSTANTS.Tag.CircleCollider);
        }
    }
}
