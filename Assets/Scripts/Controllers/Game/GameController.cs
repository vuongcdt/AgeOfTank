using Commands.Game;
using QFramework;
using Systems;
using UnityEngine;
using UnityEngine.Serialization;
using uPools;

namespace Controllers.Game
{
    public class GameController : BaseGameController
    {
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private GamePlayData gamePlayData;
        
        private void Start()
        {
            SharedGameObjectPool.Prewarm(characterPrefab, 30);

            this.RegisterEvent<Events.Events.InitCharacter>(InitPlayer);

            InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);
            InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);
            InitEnemy(CONSTANTS.CardCharacterType.FighterEnemy);

            Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
            Camera.main.transparencySortAxis = Vector3.up;
        }

        private void InitEnemy(CONSTANTS.CardCharacterType type)
        {
            this.SendCommand<SetIdEnemy>();
            var newEnemy = SharedGameObjectPool.Rent(characterPrefab, gamePlayData.pointTarget, Quaternion.identity, transform);
            newEnemy.GetComponent<Character>().InitCharacter(type, GamePlayModel.IdEnemy.Value);
            newEnemy.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }

        private void InitPlayer(Events.Events.InitCharacter e)
        {
            this.SendCommand<SetIdPlayer>();
            var newPlayer = SharedGameObjectPool.Rent(characterPrefab, gamePlayData.pointSource, Quaternion.identity, transform);
            newPlayer.GetComponent<Character>().InitCharacter(e.Type, GamePlayModel.IdPlayer.Value);
        }
    }
}