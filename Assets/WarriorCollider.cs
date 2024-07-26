
using DG.Tweening;
using UnityEngine;

public class WarriorCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var actor = GetComponentInParent<Actor>();
        if (other.CompareTag(actor.type.Contains(CONSTANTS.Tag.Player)
                ? CONSTANTS.Tag.WarriorColliderEnemy
                : CONSTANTS.Tag.WarriorColliderPlayer))
        {
            Debug.Log($"VAR {tag} {other.tag}");
            var parent = GetComponentInParent<Actor>();
            parent.transform.DOKill();
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var col = GetComponent<CircleCollider2D>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, col.radius);
    }
#endif
}