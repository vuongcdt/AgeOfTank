using Interfaces;
using QFramework;

namespace Commands
{
    public class BaseCommand : AbstractCommand
    {
        protected IGamePlayUIModel GamePlayUIModel;
        protected IGamePlayModel GamePlayModel;

        protected override void OnExecute()
        {
            GamePlayUIModel = this.GetModel<IGamePlayUIModel>();
            GamePlayModel = this.GetModel<IGamePlayModel>();
        }
    }
}