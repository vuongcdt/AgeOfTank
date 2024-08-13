using System;
using System.Collections.Generic;
using System.Threading;
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
        public Dictionary<string, CharacterStats> CharactersAttacking { get; }
        public BindableProperty<string> InitCharacterKey { get; }
        public BindableProperty<float> ProductFoodProgress { get; }
        public BindableProperty<float> HealthTargetPlayer { get; }
        public BindableProperty<float> HealthTargetEnemy { get; }
        public BindableProperty<int> FoodNum { get; }
    }

    // [Serializable]
    public class CharacterStats
    {
        public Transform Transform;
        public readonly BindableProperty<float> Health;
        public readonly int ID;
        public readonly float Damage;
        public Vector3 Target;
        public Vector3 Source;
        public readonly string Tag;
        public readonly string Name;
        public readonly bool IsPlayer;
        public bool IsAttackCharacter;
        public bool IsAttackTarget;
        public readonly Dictionary<string, CharacterStats> CharactersCanBeaten = new();
        public bool IsDeath => Health.Value < 0;
        public readonly ENUMS.CharacterType Type;
        public readonly ENUMS.CharacterTypeClass TypeClass;

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