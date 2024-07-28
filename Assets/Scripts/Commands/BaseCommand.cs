using Interfaces;
using QFramework;
using Systems;

namespace Commands
{
    public class BaseCommand : AbstractCommand
    {
        protected IGamePlayModel GamePlayModel;
        protected CharacterConfig ActorConfig;

        protected override async void OnExecute()
        {
            GamePlayModel = this.GetModel<IGamePlayModel>();
            ActorConfig = await  this.GetSystem<ConfigSystem>().GetCharacterConfig();
        }
    }
}