using System;
using UnityEngine;

namespace Controllers.Test
{
    public class Enemy :MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(tag))
            {
                return;
            }
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        // private void OnCollisionStay2D(Collision2D other)
        // {
        //     GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        // }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag(tag))
            {
                return;
            }
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }
}