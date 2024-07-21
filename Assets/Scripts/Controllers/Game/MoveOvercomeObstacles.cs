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

            if (_characterRun.Model.ID <= _characterAttract.Model.ID)
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
            var offset = (_characterRun.Model.ID % 2 == 0 ? Vector3.up : Vector3.down) * 0.5f;
            var offsetCharacter = _characterRun.transform.position.x < _characterRun.Model.Target.x - 0.5f
                ? (_characterRun.Model.IsPlayer ? Vector3.right : Vector3.left) * 0.5f
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

            if (_characterRun.Model.ID <= _characterAttract.Model.ID)
            {
                return;
            }

            transform.DOKill();

            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                _characterRun.transform.position.x,
                _characterRun.Model.Source.x,
                _characterRun.Model.Target.x,
                gamePlayData.durationMove);

            _characterRun.transform
                .DOMove(new Vector3(_characterRun.Model.Target.x, _newPointTarget.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}