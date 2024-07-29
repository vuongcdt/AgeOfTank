﻿using System.Collections.Generic;
using Controllers.NewGame;
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
        public Dictionary<string, Actor> ActorsAttacking { get; } = new();
        public Dictionary<string, Actor> PlayerAttack { get; } = new();
        public Dictionary<string, Actor> EnemyAttack { get; } = new();
        public BindableProperty<string> InitCharacterKey { get; } = new();
        public BindableProperty<float> ProductFoodProgress { get; } = new();


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