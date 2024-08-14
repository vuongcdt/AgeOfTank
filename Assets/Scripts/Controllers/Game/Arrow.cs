using System;
using UnityEngine;
using uPools;
using Utilities;

namespace Controllers.Game
{
    public class Arrow : BaseGameController
    {
        [SerializeField] private int angle = 60, trajectoryNum = 20;
        [SerializeField] private float velocity = 4, configStepDraw = 0.1f;

        private Vector3 _startPoint, _endPoint;
        private bool _isPlayer;
        private Rigidbody2D _rg;
        private float _angleRad;
        private Transform _transformCache;

        private const float GRAFITY = 9.8f;

        public void Shooting(bool isPlayer, Vector3 targetPos)
        {
            _transformCache = transform;
            _isPlayer = isPlayer;
            _rg = GetComponent<Rigidbody2D>();
            _startPoint = transform.position;
            _endPoint = targetPos;

            Shooting();
        }

        private void FixedUpdate()
        {
            var directionVector = _rg.velocity.normalized;

            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, directionVector);

            _transformCache.rotation = Quaternion.Euler(0, 0, lookRotation.eulerAngles.z);

            var pos = _transformCache.position;
            var isEnd = pos.x> _endPoint.x && pos.y< _endPoint.y;
            if (isEnd)
            {
                SharedGameObjectPool.Return(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsTargetCompetitor(other) && gameObject.activeSelf)
            {
                SharedGameObjectPool.Return(gameObject);
            }
        }

        private bool IsTargetCompetitor(Collider2D other)
        {
            var competitorTag = _isPlayer
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;
            return other.CompareTag(competitorTag);
        }

        private void Shooting()
        {
            CalcVelocity();

            var rgVelocity = Vector3.one;
            rgVelocity.x = velocity * Mathf.Cos(_angleRad);
            rgVelocity.y = velocity * Mathf.Sin(_angleRad);

            if (configStepDraw < 0)
            {
                rgVelocity *= -1;
            }

            _rg.velocity = rgVelocity;
        }


        void CalcVelocity()
        {
            var x = _endPoint.x - _startPoint.x;
            var y = _endPoint.y - _startPoint.y;

            if (x > 0)
            {
                _angleRad = Mathf.Abs(angle * Mathf.Deg2Rad);
                configStepDraw = Mathf.Abs(configStepDraw);
            }
            else
            {
                _angleRad = -Mathf.Abs(angle * Mathf.Deg2Rad);
                configStepDraw = -Mathf.Abs(configStepDraw);
            }

            var v2 = GRAFITY * x * x /
                     (2 * Mathf.Cos(_angleRad) * Mathf.Cos(_angleRad) * (x * Mathf.Tan(_angleRad) - y));
            v2 = Mathf.Abs(v2);
            velocity = Mathf.Sqrt(v2);
        }

        // private void OnDrawGizmos()
        // {
        //     CalcVelocity();
        //     Gizmos.color = Color.red;
        //     var position = _startPoint;
        //
        //
        //     for (var i = 0; i < trajectoryNum; i++)
        //     {
        //         var time = i * configStepDraw;
        //         var x = velocity * Mathf.Cos(_angleRad) * time;
        //         var y = velocity * Mathf.Sin(_angleRad) * time - 0.5f * (GRAFITY * time * time);
        //
        //         var point1 = position + new Vector3(x, y, 0);
        //
        //         time = (i + 1) * configStepDraw;
        //         x = velocity * Mathf.Cos(_angleRad) * time;
        //         y = velocity * Mathf.Sin(_angleRad) * time - 0.5f * (GRAFITY * time * time);
        //
        //         var point2 = position + new Vector3(x, y, 0);
        //
        //         Gizmos.DrawLine(point1, point2);
        //     }
        // }
    }
}