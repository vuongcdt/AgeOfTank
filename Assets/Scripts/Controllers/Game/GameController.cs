using Commands.Game;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using uPools;

namespace Controllers.Game
{
    public class GameController : BaseGameController
    {
        [SerializeField] private GameObject characterPrefab;

        [SerializeField] private AssetReference refResource = new();

        private void Start()
        {
            SharedGameObjectPool.Prewarm(characterPrefab, 30);

            this.RegisterEvent<Events.Events.InitCharacter>(e =>
            {
                this.SendCommand(new InitCharacterCommand(e.Type));
            });

            GamePlayModel.InitCharacterKey.Register(RenderCharacter);

            this.SendCommand(new InitCharacterCommand(CONSTANTS.CardCharacterType.FighterEnemy));
            this.SendCommand(new InitCharacterCommand(CONSTANTS.CardCharacterType.FighterEnemy));
            // this.SendCommand(new InitCharacterCommand(CONSTANTS.CardCharacterType.FighterEnemy));
            // this.SendCommand(new InitCharacterCommand(CONSTANTS.CardCharacterType.FighterEnemy));
            // this.SendCommand(new InitCharacterCommand(CONSTANTS.CardCharacterType.FighterEnemy));

            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;
        }

        private void RenderCharacter(string newKey)
        {
            var parent = transform;
            var newCharacter =
                SharedGameObjectPool.Rent(characterPrefab, parent.position, Quaternion.identity, parent);
            var character = newCharacter.GetComponent<Character>();

            character.InitCharacter(newKey);
        }
    }
}