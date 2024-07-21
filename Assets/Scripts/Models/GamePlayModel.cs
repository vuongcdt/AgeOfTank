using System.Collections.Generic;
using Controllers.Game;
using Interfaces;
using QFramework;

namespace Models
{
    public class GamePlayModel:AbstractModel,IGamePlayModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; } = new(0);
        public BindableProperty<int> IdEnemy { get; } = new(0);
        public Dictionary<Character, BindableProperty<Character>> Characters { get; } = new();

        protected override void OnInit()
        {
        }
    }
}