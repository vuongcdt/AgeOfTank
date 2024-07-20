using Interfaces;
using QFramework;

namespace Models
{
    public class GamePlayUIModel : AbstractModel, IGamePlayUIModel
    {
        public BindableProperty<int> Count { get; } = new();
        public BindableProperty<int> FoodNum { get; } = new(500);
        public BindableProperty<float> FoodPerSecond { get; } = new(1f);

        protected override void OnInit()
        {
            var storage = this.GetUtility<IGameStorage>();

            Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));
        }
    }
}