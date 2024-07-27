using UnityEngine;
using Utilities;

public class SameTypeCollider : MonoBehaviour
{
    private Actor _actor;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        tag = CONSTANS.Tag.SameTypeCollider;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var actorCollider = other.GetComponentInParent<Actor>();
        if (_actor.id > actorCollider.id)
        {
            return;
        }

        if (actorCollider.isAttack)
        {
            return;
        }

        if (other.CompareTag(tag) &&
            tag.Contains(CONSTANS.Tag.SameTypeCollider))
        {
            var pos = transform.position;
            _actor.StopMove();
            _actor.MoveToPoint(new Vector3(pos.x, -1f), 2f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var actorCollider = other.GetComponentInParent<Actor>();
        if (!actorCollider)
        {
            return;
        }

        if (_actor.id > actorCollider.id)
        {
            return;
        }

        if (other.CompareTag(tag) &&
            tag.Contains(CONSTANS.Tag.SameTypeCollider))
        {
            _actor.StopMove();
            _actor.MoveToPoint((_actor.type == ENUMS.CharacterType.Player ? Vector3.right : Vector3.left) * 2, 15f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var position = transform.position;
        Gizmos.DrawWireSphere(new Vector3(position.x + 0.125f, position.y), 0.125f);
        Gizmos.DrawWireSphere(new Vector3(position.x - 0.125f, position.y), 0.125f);
    }
#endif
}