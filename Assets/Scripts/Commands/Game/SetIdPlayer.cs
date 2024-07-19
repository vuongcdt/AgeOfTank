namespace Commands.Game
{
    public class SetIdPlayer : BaseCommand
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            GamePlayModel.IdPlayer.Value++;
        }
    }
}