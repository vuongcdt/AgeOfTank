using System;
using UnityEngine;
using uPools;
using Utilities;

namespace Controllers.Game
{
    public class Arrow : BaseGameController
    {
        [SerializeField] private Vector3 startPoint, endPoint;
        [SerializeField] private int angle = 60, trajectoryNum = 20;
        [SerializeField] private float velocity = 4, config = 0.1f;
        // [SerializeField] private GameObject bullet;

        private bool _isPlayer;
        private Rigidbody2D _rg;
        private float _angleRad;
        private const float GRAFITY = 9.8f;

        public void Shooting(bool isPlayer, Vector3 targetPos)
        {
            _isPlayer = isPlayer;
            _rg = GetComponent<Rigidbody2D>();
            startPoint = transform.position;
            endPoint = targetPos;

            _rg.velocity = isPlayer ? Vector3.one : new Vector3(-1, 1);

            transform.rotation = isPlayer
                ? Quaternion.AngleAxis(-45, Vector3.forward)
                : Quaternion.AngleAxis(45, Vector3.forward);

            Shooting();
        }

        private void FixedUpdate()
        {
            transform.Rotate(_rg.velocity);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsTargetCompetitor(other))
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
            CalcVeclocity();

            var rgVelocity = Vector3.one;
            rgVelocity.x = velocity * Mathf.Cos(_angleRad);
            rgVelocity.y = velocity * Mathf.Sin(_angleRad);

            if (config < 0)
            {
                rgVelocity *= -1;
            }

            _rg.velocity = rgVelocity;
        }

        void CalcVeclocity()
        {
            var x = endPoint.x - startPoint.x;
            var y = endPoint.y - startPoint.y;

            if (x > 0)
            {
                _angleRad = Mathf.Abs(angle * Mathf.Deg2Rad);
                config = Mathf.Abs(config);
            }
            else
            {
                _angleRad = -Mathf.Abs(angle * Mathf.Deg2Rad);
                config = -Mathf.Abs(config);
            }

            var v2 = GRAFITY * x * x /
                     (2 * Mathf.Cos(_angleRad) * Mathf.Cos(_angleRad) * (x * Mathf.Tan(_angleRad) - y));
            v2 = Mathf.Abs(v2);
            velocity = Mathf.Sqrt(v2);
        }

        private void OnDrawGizmos()
        {
            CalcVeclocity();
            Gizmos.color = Color.red;
            var position = startPoint;


            for (var i = 0; i < trajectoryNum; i++)
            {
                var time = i * config;
                var x = velocity * Mathf.Cos(_angleRad) * time;
                var y = velocity * Mathf.Sin(_angleRad) * time - 0.5f * (GRAFITY * time * time);

                var point1 = position + new Vector3(x, y, 0);

                time = (i + 1) * config;
                x = velocity * Mathf.Cos(_angleRad) * time;
                y = velocity * Mathf.Sin(_angleRad) * time - 0.5f * (GRAFITY * time * time);

                var point2 = position + new Vector3(x, y, 0);

                Gizmos.DrawLine(point1, point2);
            }
        }
    }
}