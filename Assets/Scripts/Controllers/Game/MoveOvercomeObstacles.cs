using DG.Tweening;
using Systems;
using UnityEngine;

namespace Controllers.Game
{
    public class MoveOvercomeObstacles : BaseGameController
    {
        [SerializeField] private GamePlayData gamePlayData;

        private Character _characterRun;
        private Character _characterAttract;
        private Vector3 _newPointTarget;

        protected override void AwaitCustom()
        {
            _characterRun = GetComponentInParent<Character>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }

            _characterAttract = other.GetComponentInParent<Character>();

            if (_characterRun.ID <= _characterAttract.ID)
            {
                return;
            }

            transform.DOKill();

            _characterRun.transform
                .DOMove(GetNewPointTarget(), gamePlayData.durationMove * 0.1f)
                .SetEase(Ease.Linear);
        }

        private Vector3 GetNewPointTarget()
        {
            var offset = (_characterRun.ID % 2 == 0 ? Vector3.up : Vector3.down) * 0.5f;
            var offsetCharacter = _characterRun.transform.position.x < _characterRun.Target.x - 0.5f
                ? (_characterRun.IsPlayer ? Vector3.right : Vector3.left) * 0.5f
                : Vector3.zero;

            _newPointTarget = _characterAttract.transform.position + offset + offsetCharacter;

            return _newPointTarget;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }

            _characterAttract = other.GetComponentInParent<Character>();
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

            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                _characterRun.transform.position.x,
                _characterRun.Source.x,
                _characterRun.Target.x,
                gamePlayData.durationMove);

            _characterRun.transform
                .DOMove(new Vector3(_characterRun.Target.x, _newPointTarget.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}