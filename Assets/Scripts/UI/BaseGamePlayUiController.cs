using Interfaces;
using QFramework;
using UnityEngine;

namespace UI
{
    public class BaseGamePlayUiController : MonoBehaviour, IController, ICanSendEvent
    {
        protected IGamePlayModel GamePlayModel;

        private void Awake()
        {
            GamePlayModel = this.GetModel<IGamePlayModel>();
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