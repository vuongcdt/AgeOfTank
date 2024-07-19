using Controllers.GamePlay;
using QFramework;

namespace Interfaces
{
    public interface IGamePlayModel:IModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> IdPlayer { get; }
        public BindableProperty<int> IdEnemy { get; }
    }
}