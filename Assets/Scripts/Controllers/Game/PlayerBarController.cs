using System;
using System.Collections;
using UnityEngine;

namespace Controllers.Game
{
    public class PlayerBarController:BaseGameController
    {
        private Rigidbody2D _rg;
        // private void Start()
        // {
        //     _rg = GetComponent<Rigidbody2D>();
        //     StartCoroutine(MoveToSource());
        // }
        //
        // private IEnumerator MoveToSource()
        // {
        //     yield return new WaitForSeconds(0.1f);
        //     // _rg.velocity = new Vector2(-2, 0);
        //     // _rg.AddForce(new Vector2(-2, 0));
        // }
    }
}