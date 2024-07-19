using DG.Tweening;
using UnityEngine;

namespace Controllers.Game
{
    public class MoveOvercomeObstacles : BaseGameController
    {
        private Character _character;
        private Character _obstacle;
        private Vector3 _newPointTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(tag))
            {
                return;
            }

            _character = GetComponentInParent<Character>();
            _obstacle = other.gameObject.GetComponentInParent<Character>();
            if (_character.ID > _obstacle.ID)
            {
                return;
            }

            var offset = (_character.ID % 2 == 0 ? Vector3.up : Vector3.down) * 0.5f;
            _newPointTarget = _obstacle.transform.position + offset;

            _character.transform
                .DOMove(_newPointTarget, 0.5f)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(tag) )
            {
                return;
            }

            _character.transform
                .DOMove(new Vector3(_character.PointTarget.x, _newPointTarget.y), 9)
                .SetEase(Ease.Linear);
        }
    }
}