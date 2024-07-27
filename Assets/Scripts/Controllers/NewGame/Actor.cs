using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Controllers.NewGame
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private TextMesh idText;
        [SerializeField] private SpriteRenderer avatar;
        
        public int id;
        public ENUMS.CharacterType type;
        public ENUMS.CharacterTypeClass typeClass;
        public bool isAttack;
        public bool isPlayer;
        public Vector3 start, end;

        private void Awake()
        {
            tag = type == ENUMS.CharacterType.Enemy
                ? CONSTANS.Tag.Enemy
                : CONSTANS.Tag.Player;
            name = $"{type.ToString()}{id}";
            isPlayer = type == ENUMS.CharacterType.Player;
            idText.text = id.ToString();
            avatar.flipX = !isPlayer;
        }

        public void SetSameTypeColliderHunter()
        {
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
}