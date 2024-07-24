using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Interfaces
{
    public interface IGamePlayModel : IModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; }
        public BindableProperty<int> IdEnemy { get; }
        public Dictionary<string, CharacterStats> Characters { get; }
        public BindableProperty<string> InitCharacterKey { get; }
        public BindableProperty<float> ProductFoodProgress { get; }
        public BindableProperty<int> FoodNum { get; }
    }

    public class CharacterStats
    {
        public BindableProperty<float> Health;
        public int ID;
        public float Damage;
        public Vector3 Target;
        public Vector3 Source;
        public bool IsPlayer;
        public string Tag;
        public string Name;
        
        public bool IsDeath => Health.Value < 0;
        public CONSTANTS.CardCharacterType Type;

        public CharacterStats(float health, int id, float damage, Vector3 target, Vector3 source,
             string tag, string name, CONSTANTS.CardCharacterType type)
        {
            Health = new BindableProperty<float>(health);
            ID = id;
            Damage = damage;
            Target = target;
            Source = source;
            IsPlayer = (int)type < 3;
            Tag = tag;
            Name = name;
            Type = type;
        }
    }
}