using System;
using Commands.Game;
using Controllers.Game;
using QFramework;
using UnityEngine;
using uPools;

namespace Controllers.GamePlay
{
    public class GameController : BaseGameController
    {
        [SerializeField] private Transform pointSource, pointTarget;
        [SerializeField] private Character character;

        private void Start()
        {
            SharedGameObjectPool.Prewarm(character.gameObject, 30);

            this.RegisterEvent<Events.Events.InitCharacter>(InitPlayer);

            InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);
            // InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);
            // InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);

            Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
            Camera.main.transparencySortAxis = Vector3.up;
        }

        private void InitEnemy(CONSTANTS.CardCharacterType type)
        {
            var newEnemy = SharedGameObjectPool.Rent(character, pointTarget.position, Quaternion.identity, transform);
            this.SendCommand<SetIdEnemy>();
            newEnemy.InitCharacter(type, GamePlayModel.IdEnemy.Value);
            newEnemy.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            newEnemy.MoveCharacter(pointSource.position);
        }

        private void InitPlayer(Events.Events.InitCharacter e)
        {
            var newPlayer = SharedGameObjectPool.Rent(character, pointSource.position, Quaternion.identity, transform);
            this.SendCommand<SetIdPlayer>();
            newPlayer.InitCharacter(e.Type, GamePlayModel.IdPlayer.Value);
            newPlayer.MoveCharacter(pointTarget.position);
        }
    }
}