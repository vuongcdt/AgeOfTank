using System.Threading.Tasks;
using Commands.Game;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using uPools;
using Utilities;

namespace Controllers.Game
{
    public class GameController : BaseGameController
    {
        [SerializeField] private Button resetBtn;
        [SerializeField] private Button playBtn;
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private int[] playersList;
        [SerializeField] private int[] enemiesList;
        [SerializeField] private int[] playersHunterList;
        [SerializeField] private int[] enemiesHunterList;

        private void Start()
        {
            resetBtn.onClick.RemoveAllListeners();
            resetBtn.onClick.AddListener(OnReset);
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(OnPlay);

            SharedGameObjectPool.Prewarm(characterPrefab, 30);

            this.RegisterEvent<InitCharacter>(e => this.SendCommand(new InitCharacterCommand(e.TypeClass)));

            GamePlayModel.InitCharacterKey.Register(RenderCharacter);

            SpawnEnemies();

            SpawnPlayer();

            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;
        }

        private void OnPlay()
        {
            Time.timeScale = 1;
        }

        private async void SpawnPlayer()
        {
            foreach (var value in playersList)
            {
                foreach (var i in new int[value])
                {
                    this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.Fighter));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }

            foreach (var value in playersHunterList)
            {
                foreach (var i in new int[value])
                {
                    this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.Hunter));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }
        }

        private async void SpawnEnemies()
        {
            foreach (var value in enemiesList)
            {
                foreach (var i in new int[value])
                {
                    this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.FighterEnemy));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }

            foreach (var value in enemiesHunterList)
            {
                foreach (var i in new int[value])
                {
                    this.SendCommand(new InitCharacterCommand(ENUMS.CharacterTypeClass.FighterEnemy));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }
        }

        private void OnReset()
        {
            foreach (var pair in GamePlayModel.Characters)
            {
                SharedGameObjectPool.Return(pair.Value.GameObject);
            }

            SpawnEnemies();
            SpawnPlayer();
        }

        private void RenderCharacter(string newKey)
        {
            var parent = transform;
            var newCharacter =
                SharedGameObjectPool.Rent(characterPrefab, parent.position, Quaternion.identity, parent);

            var character = newCharacter.GetComponent<Character>();

            character.RenderCharacter(newKey);
        }
    }
}