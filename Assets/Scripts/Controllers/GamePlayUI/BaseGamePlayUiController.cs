using Interfaces;
using QFramework;
using UnityEngine;

namespace Controllers.GamePlayUI
{
    public class BaseGamePlayUiController : MonoBehaviour, IController, ICanSendEvent
    {
        protected IGamePlayUIModel GamePlayUIModel;

        private void Awake()
        {
            GamePlayUIModel = this.GetModel<IGamePlayUIModel>();
        }

        public IArchitecture GetArchitecture()
        {
            return App.Interface;
        }

        private void OnDestroy()
        {
            GamePlayUIModel = null;
        }
    }
}