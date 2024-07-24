using Interfaces;
using Models;
using QFramework;
using Systems;
using Utilities;

public class App : Architecture<App>
{
    protected override void Init()
    {
        RegisterModel<IGamePlayModel>(new GamePlayModel());
        
        RegisterSystem<IGameSystem>(new GameSystem());
        RegisterSystem(new ConfigSystem());
        RegisterSystem(new FoodFactorySystem());
        
        RegisterUtility<IGameStorage>(new GameStorage());
    }
}