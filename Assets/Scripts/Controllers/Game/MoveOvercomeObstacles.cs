using DG.Tweening;
using UnityEngine;

namespace Controllers.Game
{
    public class MoveOvercomeObstacles : BaseGameController
    {
        private Character _characterRun;
        private Character _characterAttract;
        private Vector3 _newPointTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }

            _characterRun = GetComponentInParent<Character>();
            _characterAttract = other.GetComponentInParent<Character>();
            
            if (_characterRun.ID <= _characterAttract.ID)
            {
                return;
            }
            transform.DOKill();
            
            var offset = (_characterRun.ID % 2 == 0 ? Vector3.up : Vector3.down) * 0.5f;
            _newPointTarget = _characterAttract.transform.position + offset;

            _characterRun.transform
                .DOMove(_newPointTarget, _characterRun.DurationMove * 0.1f)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }
            if (_characterRun.ID <= _characterAttract.ID)
            {
                return;
            }
            transform.DOKill();
            Debug.Log(_characterRun.name);
            _characterRun.transform
                .DOMove(new Vector3(_characterRun.PointTarget.x, _newPointTarget.y), _characterRun.DurationMove)
                .SetEase(Ease.Linear);
        }
    }
}