using QFramework;

namespace Interfaces
{
    public interface IGamePlayUIModel:IModel
    {
        public BindableProperty<int> Count { get; }
        public BindableProperty<int> FoodNum { get; }
        public BindableProperty<float> FoodPerSecond { get; }
    }
}