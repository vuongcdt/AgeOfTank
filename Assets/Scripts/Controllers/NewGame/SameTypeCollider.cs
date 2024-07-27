using Controllers.Game;
using DG.Tweening;
using QFramework;
using Systems;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
//     public class SameTypeCollider : BaseGameController
//     {
//         private Actor _actor;
//         private CharacterConfig _characterConfig;
//         private Actor _actorObstacle;
//
//         private async void Awake()
//         {
//             _actor = GetComponentInParent<Actor>();
//             tag = CONSTANS.Tag.SameTypeCollider;
//         }
//
//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             var actorEnter = other.GetComponentInParent<Actor>();
//             if (_actor.id <= actorEnter.id)
//             {
//                 return;
//             }
//
//             if (actorEnter.isAttack)
//             {
//                 return;
//             }
//
//             if (other.CompareTag(tag) &&
//                 tag.Contains(CONSTANS.Tag.SameTypeCollider))
//             {
//                 _actorObstacle = actorEnter;
//                 MoveOvercomeObstacles();
//             }
//         }
//
//         private async void MoveOvercomeObstacles()
//         {
//             _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
//             _actor.StopMove();
//             // _actor.MoveToPoint(new Vector3(pos.x, -1f), 2f);
//
//             var offset = (_actor.id % 2 == 0 ? Vector3.up : Vector3.down) * 1f;
//             var offsetCharacter = (_actor.isPlayer ? Vector3.right : Vector3.left) * 0.5f;
//             var pos = _actor.transform.position;
//
//             var newPointTarget = pos + offset + offsetCharacter;
//
//             _actor.MoveToPoint(newPointTarget, _characterConfig.durationMove * 0.1f);
//         }
//
//         private void OnTriggerStay2D(Collider2D other)
//         {
//             if (other.CompareTag(tag) &&
//                 tag.Contains(CONSTANS.Tag.SameTypeCollider))
//             {
//                 return;
//             }
//
//             var actorStay = other.GetComponentInParent<Actor>();
//             if (!actorStay)
//             {
//                 return;
//             }
//
//             if (_actor.id <= actorStay.id)
//             {
//                 return;
//             }
//
//             if (_actor.isPlayer)
//             {
//                 Debug.Log($"actorStay {_actor.name} {actorStay.name}");
//             }
//
//             _actorObstacle = actorStay;
//         }
//
//         private void OnTriggerExit2D(Collider2D other)
//         {
//             var actorExit = other.GetComponentInParent<Actor>();
//             if (!actorExit)
//             {
//                 return;
//             }
//
//             // if (actorExit.id != _actorObstacle.id)
//             // {
//             //     return;
//             // }
//
//             if (_actor.id <= actorExit.id)
//             {
//                 return;
//             }
//
//             if (other.CompareTag(tag) &&
//                 tag.Contains(CONSTANS.Tag.SameTypeCollider))
//             {
//                 if (_actor.isPlayer)
//                 {
//                     Debug.Log($"VAR {_actor.name} {actorExit.name} {_actorObstacle.name}");
//                     Time.timeScale = 0;
//                 }
//
//                 _actor.StopMove();
//                 // _actor.MoveToPoint((_actor.type == ENUMS.CharacterType.Player ? Vector3.right : Vector3.left) * 2, 15f);
//                 MoveToTarget();
//             }
//         }
//
//         private async void MoveToTarget()
//         {
//             _characterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
//
//             var pos = _actor.transform.position;
//             var durationMoveToTarget = Utils.GetDurationMoveToTarget(
//                 pos.x,
//                 _actor.start.x,
//                 _actor.end.x,
//                 _characterConfig.durationMove);
//             _actor.MoveToPoint(new Vector3(_actor.end.x, pos.y), durationMoveToTarget);
//
//             // _transformRun
//             //     .DOMove(new Vector3(_characterRun.Stats.Target.x, pos.y), durationMoveToTarget)
//             //     .SetEase(Ease.Linear);
//         }
//
// #if UNITY_EDITOR
//         private void OnDrawGizmos()
//         {
//             Gizmos.color = Color.green;
//             var position = transform.position;
//             Gizmos.DrawWireSphere(new Vector3(position.x + 0.125f, position.y), 0.125f);
//             Gizmos.DrawWireSphere(new Vector3(position.x - 0.125f, position.y), 0.125f);
//         }
// #endif
//     }

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