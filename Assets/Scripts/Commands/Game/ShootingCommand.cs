using Controllers.Game;
using Interfaces;
using UnityEngine;
using uPools;

namespace Commands.Game
{
    public class ShootingCommand : BaseCommand
    {
        private readonly string _keyAttack;
        private Vector3 _targetPos;
        private CharacterStats _statsAttack;

        public ShootingCommand(string keyAttack, Vector3 targetPos)
        {
            _keyAttack = keyAttack;
            _targetPos = targetPos;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            _statsAttack = GamePlayModel.CharactersAttacking[_keyAttack];
            Shooting();
        }

        private void Shooting()
        {
            var arrowPrefab = CharacterConfig.unitConfigs[(int)_statsAttack.TypeClass].prefabArrow;
            var arrow = SharedGameObjectPool.Rent(arrowPrefab, _statsAttack.Transform);
            arrow.transform.position = _statsAttack.Transform.position;
            arrow.GetComponent<Arrow>().Shooting(_statsAttack.IsPlayer,_targetPos);
        }
    }
}