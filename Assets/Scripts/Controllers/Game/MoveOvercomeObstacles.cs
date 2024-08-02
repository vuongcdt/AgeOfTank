using System;
using DG.Tweening;
using QFramework;
using Systems;
using UnityEngine;
using Utilities;

namespace Controllers.Game
{
    public class MoveOvercomeObstacles : BaseGameController
    {
        private Character _characterRun;
        private Character _characterObstacle;
        private Transform _transformRun;

        private void Start()
        {
            _characterRun = GetComponentInParent<Character>();
            _transformRun = _characterRun.transform;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(tag) || !tag.Contains(CONSTANS.Tag.CircleCollider) ||
                !other.tag.Contains(CONSTANS.Tag.CircleCollider))
            {
                return;
            }

            _characterObstacle = other.GetComponentInParent<Character>();

            if (_characterRun.Stats.ID <= _characterObstacle.Stats.ID)
            {
                return;
            }

            if (!_characterRun.IsMoveTarget)
            {
                return;
            }

            MoveNewPoint();
        }

        private void MoveNewPoint()
        {
            var offset = (_characterRun.Stats.ID % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
            var offsetCharacter = (_characterRun.Stats.IsPlayer ? Vector3.right : Vector3.left) * 0.5f;
            var posObstacle = _characterObstacle.transform.position;

            var newPointTarget = posObstacle + offset + offsetCharacter;

            _transformRun
                .DOMove(newPointTarget, ActorConfig.durationMove * 0.1f)
                .SetEase(Ease.Linear);
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(tag) || !tag.Contains(CONSTANS.Tag.CircleCollider) ||
                !other.tag.Contains(CONSTANS.Tag.CircleCollider))
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
            if (!other.CompareTag(tag))
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

            _transformRun.DOKill();
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            var posRun = _transformRun.position;
            var durationMoveToTarget = Utils.GetDurationMoveToTarget(
                posRun.x,
                _characterRun.Stats.Source.x,
                _characterRun.Stats.Target.x,
                ActorConfig.durationMove);

            _transformRun
                .DOMove(new Vector3(_characterRun.Stats.Target.x, posRun.y), durationMoveToTarget)
                .SetEase(Ease.Linear);
        }
    }
}