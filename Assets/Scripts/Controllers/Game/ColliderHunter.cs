using UnityEngine;

namespace Controllers.Game
{
    public class ColliderHunter:BaseGameController
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (name.Contains("5"))
            {
                Debug.Log($"{name} {tag} {other.tag}");
            }
        }
    }
}