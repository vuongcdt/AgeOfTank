using Interfaces;
using Models;
using Mono;
using QFramework;
using UnityEngine;

namespace Systems
{
    public class FoodFactorySystem : AbstractSystem
    {
        private float _timeProductRemain;
        private IGamePlayModel _gamePlayModel;

        protected override void OnInit()
        {
            _gamePlayModel = this.GetModel<IGamePlayModel>();
            var factorySystem = new GameObject("FactorySystem").AddComponent<FactorySystemMono>();
            factorySystem.OnUpdate += Process;
        }

        private void Process()
        {
            _timeProductRemain += Time.deltaTime;
            _gamePlayModel.ProductFoodProgress.Value = Mathf.Clamp(_timeProductRemain / (1 / 0.44f), 0, 1);
            
            if (_timeProductRemain >= 1 / 0.44f)
            {
                _gamePlayModel.FoodNum.Value++;
                _timeProductRemain = 0;
            }
        }
    }
}