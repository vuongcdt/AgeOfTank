using System;
using DG.Tweening;
using UnityEngine;

public class SameTypeCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var actor = GetComponentInParent<Actor>();
        if (GetInstanceID() > other.GetInstanceID())
        {
            return;
        }

        if (other.CompareTag(tag) &&
            tag.Contains(CONSTANTS.Tag.SameTypeCollider))
        {
            var pos = transform.position;
            actor.transform.DOKill();
            actor.transform.DOMove(new Vector3(pos.x - 0.5f, -1.5f), 2f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var actor = GetComponentInParent<Actor>();
        if (GetInstanceID() > other.GetInstanceID())
        {
            return;
        }

        if (other.CompareTag(tag) &&
            tag.Contains(CONSTANTS.Tag.SameTypeCollider))
        {
            Debug.Log($"EXIT {actor.tag} {tag} {other.tag}");
            actor.transform.DOMove(Vector3.left * 2, 10f);
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