using Interfaces;
using QFramework;
using UnityEngine;

namespace Controllers.Game
{
    public class BaseGameController : MonoBehaviour, IController
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