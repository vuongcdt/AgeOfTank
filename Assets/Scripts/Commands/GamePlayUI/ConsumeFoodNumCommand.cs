namespace Commands.GamePlayUI
{
    public class ConsumeFoodNumCommand : BaseCommand
    {
        private int _value;

        public ConsumeFoodNumCommand(int value)
        {
            _value = value;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            GamePlayModel.FoodNum.Value -= _value;
        }
    }
}