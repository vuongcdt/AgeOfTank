using System.Threading.Tasks;
using Commands.Game;
using Cysharp.Threading.Tasks;
using QFramework;
using TMPro;
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
        [SerializeField] private int healthTargetPlayer = 100;
        [SerializeField] private int healthTargetEnemy = 10;
        [SerializeField] private Slider healthTargetPlayerSlider;
        [SerializeField] private Slider healthTargetEnemySlider;
        [SerializeField] private TMP_Text healthTargetPlayerText;
        [SerializeField] private TMP_Text healthTargetEnemyText;


        private void Start()
        {
            resetBtn.onClick.RemoveAllListeners();
            resetBtn.onClick.AddListener(OnReset);
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(OnPlay);

            SharedGameObjectPool.Prewarm(characterPrefab, 30);

            this.RegisterEvent<InitCharacter>(e => this.SendCommand(new InitCharacterCommand(e.TypeClass)));

            GamePlayModel.InitCharacterKey.Register(RenderCharacter);
            
            GamePlayModel.HealthTargetPlayer.Register(newValue =>
            {
                if (newValue <= 0)
                {
                    SetGameWin();
                }
                healthTargetPlayerSlider.value = newValue / healthTargetPlayer;
                healthTargetPlayerText.text = newValue.ToString("0");
            });
            
            GamePlayModel.HealthTargetEnemy.Register(newValue =>
            {
                if (newValue <= 0)
                {
                    SetGameOver();
                }
                healthTargetEnemyText.text = newValue.ToString("0");
                healthTargetEnemySlider.value = newValue / healthTargetEnemy;
            });
            
            GamePlayModel.HealthTargetPlayer.Value = healthTargetPlayer;
            GamePlayModel.HealthTargetEnemy.Value = healthTargetEnemy;

            SpawnEnemies();

            SpawnPlayer();

            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;
        }

        private void SetGameOver()
        {
            Debug.Log("Game Over");
            Time.timeScale = 0;
        }

        private void SetGameWin()
        {
            Debug.Log("GAME WIN");
            Time.timeScale = 0;
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