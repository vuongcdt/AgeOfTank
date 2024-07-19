namespace Commands.Game
{
    public class SetIdEnemy : BaseCommand
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            GamePlayModel.IdEnemy.Value++;
        }
    }
}