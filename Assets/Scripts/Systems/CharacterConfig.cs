using System;
using UnityEngine;

namespace Systems
{
    [CreateAssetMenu(menuName = "CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public Vector3 pointSource, pointTarget;
        public int durationMove = 10;
        public float speed = 0.5f;
        public float attackTime = 1f;
        public float mass = 200;
        public UnitConfig[] unitConfigs;
    }

    [Serializable]
    public class UnitConfig
    {
        public float health;
        public float damage;
        public Sprite imgAvatar;
        public int foodNum;
    }
}