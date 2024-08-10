using System.Collections.Generic;
using Controllers.Game;
using Interfaces;
using QFramework;

namespace Models
{
    public class GamePlayModel : AbstractModel, IGamePlayModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; } = new(0);
        public BindableProperty<int> IdEnemy { get; } = new(0);
        public BindableProperty<int> FoodNum { get; } = new(500);
        public Dictionary<string, CharacterStats> Characters { get; } = new();
        public Dictionary<string, Character> CharactersAttacking { get; } = new();
        public BindableProperty<string> InitCharacterKey { get; } = new();
        public BindableProperty<float> ProductFoodProgress { get; } = new();
        public BindableProperty<float> HealthTargetPlayer { get; } = new();
        public BindableProperty<float> HealthTargetEnemy { get; } = new();


        protected override void OnInit()
        {
        }

        private void SpawnUnit()
        {
            // Characters.Add();
            // event
        }
    }
}