namespace Commands.GamePlayUICommands
{
    public class AddFoodCommand:BaseCommand
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            AddFood();
        }

        private void AddFood()
        {
            GamePlayUIModel.FoodNum.Value++;
        }
    }
}