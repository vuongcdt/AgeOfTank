using System;
using UnityEngine;

namespace Controllers.Test
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Vector3 target = Vector3.right * 3;
        [SerializeField] private int mass = 1;

        private Vector3 _dir;
        private Rigidbody2D _rg;

        private void Start()
        {
            _rg = GetComponent<Rigidbody2D>();
            _rg.mass = 1;
        }

        private void FixedUpdate()
        {
            _dir = (target - transform.position).normalized / 6;
            _rg.AddForce(_dir);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.CompareTag(tag))
            {
                _rg.mass = mass;
                // _rg.velocity = Vector3.zero;
            }
            // _rg.mass = 2;
        }


        // private void OnCollisionExit2D(Collision2D other)
        // {
        //     _dir = (target - transform.position).normalized / 6;
        //     _rg.velocity = _dir;
        // }
    }
}