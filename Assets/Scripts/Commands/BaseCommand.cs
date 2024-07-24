using Interfaces;
using QFramework;
using Systems;

namespace Commands
{
    public class BaseCommand : AbstractCommand
    {
        protected IGamePlayModel GamePlayModel;
        protected CharacterConfig CharacterConfig;

        protected override async void OnExecute()
        {
            GamePlayModel = this.GetModel<IGamePlayModel>();
            CharacterConfig = await this.GetSystem<ConfigSystem>().GetCharacterConfig();
        }
    }
}