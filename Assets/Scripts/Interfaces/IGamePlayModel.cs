using System;
using System.Collections.Generic;
using Controllers.Game;
using QFramework;
using UnityEngine;
using Utilities;

namespace Interfaces
{
    public interface IGamePlayModel : IModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; }
        public BindableProperty<int> IdEnemy { get; }
        public Dictionary<string, CharacterStats> Characters { get; }
        public Dictionary<string, Character> CharactersAttacking { get; }
        public BindableProperty<string> InitCharacterKey { get; }
        public BindableProperty<float> ProductFoodProgress { get; }
        public BindableProperty<int> FoodNum { get; }
    }

    // [Serializable]
    public class CharacterStats
    {
        public GameObject GameObject;
        public BindableProperty<float> Health;
        public int ID;
        public float Damage;
        public Vector3 Target;
        public Vector3 Source;
        public string Tag;
        public string Name;
        public bool IsPlayer;
        public bool IsAttack;
        public Dictionary<string, Character> CharactersCanBeaten = new();

        public bool IsDeath => Health.Value < 0;
        public ENUMS.CharacterType Type;
        public ENUMS.CharacterTypeClass TypeClass;

        public CharacterStats(float health, int id, float damage, Vector3 target, Vector3 source,
            string tag, string name, ENUMS.CharacterTypeClass typeClass, ENUMS.CharacterType type)
        {
            Health = new BindableProperty<float>(health);
            ID = id;
            Damage = damage;
            Target = target;
            Source = source;
            IsPlayer = (int)typeClass < 3;
            Tag = tag;
            Name = name;
            TypeClass = typeClass;
            Type = type;
        }
    }
}