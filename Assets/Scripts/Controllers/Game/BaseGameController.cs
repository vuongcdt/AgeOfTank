using Interfaces;
using QFramework;
using Systems;
using UnityEngine;

namespace Controllers.Game
{
    public class BaseGameController : MonoBehaviour, IController, ICanSendEvent
    {
        protected IGamePlayModel GamePlayModel;
        protected CharacterConfig ActorConfig;

        private async void Awake()
        {
            GamePlayModel = this.GetModel<IGamePlayModel>();
            var configSystem = this.GetSystem<ConfigSystem>();
            ActorConfig = await configSystem.GetCharacterConfig();
            AwaitCustom();
        }

        protected virtual void AwaitCustom()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return App.Interface;
        }

        private void OnDestroy()
        {
            GamePlayModel = null;
        }
    }
}