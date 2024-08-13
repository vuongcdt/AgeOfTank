using UnityEngine;

namespace Controllers.Game
{
    public class ThrowObject : MonoBehaviour
    {
        // [SerializeField] private Transform startPoint, endPoint;
        // [SerializeField] private int angle = 60, trajectoryNum = 20;
        // [SerializeField] private float velocity = 10, config = 0.1f;
        // [SerializeField] private GameObject bullet;
        // [SerializeField] private int _count = 30;
        //
        // private float _angleRad;
        // private const float GRAFITY = 10f;
        // private int count = 0;
        //
        // private void FixedUpdate()
        // {
        //     count--;
        //     if (count < 0)
        //     {
        //         Shooting();
        //         count = _count;
        //     }
        // }
        //
        // private void Shooting()
        // {
        //     CalcVeclocity();
        //     var bulletInstantiate = Instantiate(bullet, startPoint.position, Quaternion.identity);
        //
        //     var Velocity = Vector3.one;
        //     Velocity.x = velocity * Mathf.Cos(_angleRad);
        //     Velocity.y = velocity * Mathf.Sin(_angleRad);
        //     Vector3 force = Velocity * 50;
        //
        //     if (config < 0)
        //     {
        //         force *= -1;
        //         Velocity *= -1;
        //     }
        //
        //     bulletInstantiate.GetComponent<Rigidbody2D>().velocity = Velocity;
        //     // bulletInstantiate.GetComponent<Rigidbody2D>().AddForce(force);
        //     // Destroy(bulletInstantiate, 5);
        // }
        //
        // void CalcVeclocity()
        // {
        //     var x = endPoint.position.x - startPoint.position.x;
        //     var y = endPoint.position.y - startPoint.position.y;
        //
        //     if (x > 0)
        //     {
        //         _angleRad = Mathf.Abs(angle * Mathf.Deg2Rad);
        //         config = Mathf.Abs(config);
        //     }
        //     else
        //     {
        //         _angleRad = -Mathf.Abs(angle * Mathf.Deg2Rad);
        //         config = -Mathf.Abs(config);
        //     }
        //
        //     var v2 = GRAFITY * x * x /
        //              (2 * Mathf.Cos(_angleRad) * Mathf.Cos(_angleRad) * (x * Mathf.Tan(_angleRad) - y));
        //     v2 = Mathf.Abs(v2);
        //     // velocity = Mathf.Sqrt(v2);
        // }
        //
        // private void OnDrawGizmos()
        // {
        //     CalcVeclocity();
        //     Gizmos.color = Color.red;
        //     var position = startPoint.position;
        //
        //
        //     for (var i = 0; i < trajectoryNum; i++)
        //     {
        //         var time = i * config;
        //         var x = velocity * Mathf.Cos(_angleRad) * time;
        //         var y = velocity * Mathf.Sin(_angleRad) * time - 0.5f * (GRAFITY * time * time);
        //
        //         var point1 = position + new Vector3(x, y, 0);
        //
        //         time = (i + 1) * config;
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