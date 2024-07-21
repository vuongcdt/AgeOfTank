using System.Collections.Generic;
using Controllers.Game;
using QFramework;
using UnityEngine;

namespace Interfaces
{
    public interface IGamePlayModel : IModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; }
        public BindableProperty<int> IdEnemy { get; }
        public Dictionary<string, CharacterModel> Characters { get; }
    }

    public class CharacterModel
    {
        public BindableProperty<float> Health;
        public int ID;
        public float Damage;
        public Vector3 Target;
        public Vector3 Source;
        public bool IsPlayer;
        public bool IsDeath => Health.Value < 0;
        public CONSTANTS.CardCharacterType Type;

        public CharacterModel(float health, int id, float damage, Vector3 target, Vector3 source,
            CONSTANTS.CardCharacterType type)
        {
            Health = new BindableProperty<float>(health);
            ID = id;
            Damage = damage;
            Target = target;
            Source = source;
            Type = type;
            IsPlayer = (int)type < 3;
        }
    }
}