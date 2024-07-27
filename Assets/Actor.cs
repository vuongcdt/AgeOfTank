using DG.Tweening;
using UnityEngine;
using Utilities;

public class Actor : MonoBehaviour
{
    public int id;
    public ENUMS.CharacterType type;
    public ENUMS.CharacterTypeClass typeClass;
    public bool isMove;
    public bool isAttack;

    private void Awake()
    {
        tag = type == ENUMS.CharacterType.Enemy
            ? CONSTANS.Tag.Enemy
            : CONSTANS.Tag.Player;
        name = $"{type.ToString()} {id}";
    }

    public void StopMove()
    {
        transform.DOKill();
    }

    public void Attack()
    {
        transform.DOKill();
        isAttack = true;
    }

    public void MoveToPoint(Vector3 pos, float time)
    {
        transform.DOMove(pos, time);
    }
}