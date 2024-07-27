using Commands.Game;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using uPools;

namespace Controllers.Game
{
    public class GameController : BaseGameController
    {
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private int playerTotal = 3;
        [SerializeField] private int enemyTotal = 3;

        private void Start()
        {
            SharedGameObjectPool.Prewarm(characterPrefab, 30);

            this.RegisterEvent<Events.Events.InitCharacter>(e =>
            {
                this.SendCommand(new InitCharacterCommand(e.TypeClass));
            });

            GamePlayModel.InitCharacterKey.Register(RenderCharacter);

            foreach (var i in new int[enemyTotal])
            {
                this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.FighterEnemy));
            }

            foreach (var i in new int[playerTotal])
            {
                this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.Fighter));
            }

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