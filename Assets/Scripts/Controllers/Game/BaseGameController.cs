﻿using Interfaces;
using QFramework;
using Systems;
using UnityEngine;

namespace Controllers.Game
{
    public class BaseGameController : MonoBehaviour, IController, ICanSendEvent
    {
        protected IGamePlayModel GamePlayModel;
        protected CharacterConfig CharacterConfig;

        private async void Awake()
        {
            GamePlayModel = this.GetModel<IGamePlayModel>();
            CharacterConfig = await  this.GetSystem<ConfigSystem>().GetCharacterConfig();
            AwakeCustom();
        }

        protected virtual void AwakeCustom()
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