using DG.Tweening;
using UnityEngine;
using Utilities;

public class HunterCollider : MonoBehaviour
{
    private Actor _actor;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _actor = GetComponentInParent<Actor>();

        var isHunterClass = (int)_actor.typeClass % 3 == 1;
        gameObject.SetActive(isHunterClass);

        tag = _actor.type == ENUMS.CharacterType.Player
            ? CONSTANS.Tag.HunterColliderPlayer
            : CONSTANS.Tag.HunterColliderEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var opposingTag = _actor.type == ENUMS.CharacterType.Player
            ? CONSTANS.Tag.WarriorColliderEnemy
            : CONSTANS.Tag.WarriorColliderPlayer;

        if (other.CompareTag(opposingTag))
        {
            _actor.Attack();
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