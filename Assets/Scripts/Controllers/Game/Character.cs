using System;
using DG.Tweening;
using Systems;
using UnityEngine;

namespace Controllers.Game
{
    public class Character : BaseGameController
    {
        [SerializeField] private SpriteRenderer avatar;
        [SerializeField] private GameUIData gameUIData;
        [SerializeField] private int durationMove = 10;

        private Vector3 _pointTarget;
        private CONSTANTS.CardCharacterType _type;
        private int _id;

        public int ID => _id;
        public Vector3 PointTarget => _pointTarget;

        public void InitCharacter(CONSTANTS.CardCharacterType type, int id)
        {
            _type = type;
            avatar.sprite = gameUIData.imgAvatar[(int)type];
            tag = (int)type < 3 ? "Player" : "Enemy";
            name = $"{tag} {id}";
            _id = id;
        }

        public void MoveCharacter(Vector3 target)
        {
            _pointTarget = target;
            transform
                .DOMove(target, durationMove)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tagOpposition = (int)_type < 3 ? "Enemy" : "Player";

            if (other.CompareTag(tagOpposition))
            {
                transform.DOPause();
            }
        }
    }
}