using Interfaces;
using Models;
using QFramework;
using Systems;
using Utilities;

public class App : Architecture<App>
{
    protected override void Init()
    {
        this.RegisterModel<IGamePlayUIModel>(new GamePlayUIModel());
        this.RegisterModel<IGamePlayModel>(new GamePlayModel());
        
        this.RegisterSystem<IGameSystem>(new GameSystem());
        this.RegisterUtility<IGameStorage>(new GameStorage());
    }
}