using System;
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
    [Serializable]
    public class SpawnCharacter
    {
        public int num;
        public ENUMS.CharacterTypeClass type;
    }
    public class GameController : BaseGameController
    {
        [SerializeField] private Button resetBtn;
        [SerializeField] private Button playBtn;
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private SpawnCharacter[] playersList;
        [SerializeField] private SpawnCharacter[] enemiesList;
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

            GamePlayModel.HealthTargetPlayer.Register(OnHealthTargetPlayer);

            GamePlayModel.HealthTargetEnemy.Register(OnHeaalthTargetEnemy);

            GamePlayModel.HealthTargetPlayer.Value = healthTargetPlayer;
            GamePlayModel.HealthTargetEnemy.Value = healthTargetEnemy;

            SpawnEnemies();

            SpawnPlayer();

            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;
        }

        private void OnHeaalthTargetEnemy(float newValue)
        {
            if (newValue <= 0)
            {
                Debug.Log("Game Over");
                Time.timeScale = 0;
            }

            healthTargetEnemyText.text = newValue.ToString("0");
            healthTargetEnemySlider.value = newValue / healthTargetEnemy;
        }

        private void OnHealthTargetPlayer(float newValue)
        {
            if (newValue <= 0)
            {
                Debug.Log("GAME WIN");
                Time.timeScale = 0;
            }

            healthTargetPlayerSlider.value = newValue / healthTargetPlayer;
            healthTargetPlayerText.text = newValue.ToString("0");
        }

        private void OnPlay()
        {
            Time.timeScale = 1;
        }

        private async void SpawnPlayer()
        {
            foreach (var value in playersList)
            {
                foreach (var i in new int[value.num])
                {
                    this.SendCommand(new InitCharacterCommand(value.type));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }
            var arrowPrefab = CharacterConfig.unitConfigs[2].prefabArrow;
            SharedGameObjectPool.Prewarm(arrowPrefab, 30);
        }

        private async void SpawnEnemies()
        {
            foreach (var value in enemiesList)
            {
                foreach (var i in new int[value.num])
                {
                    this.SendCommand(new InitCharacterCommand(value.type));
                    await UniTask.WaitForSeconds(0.1f);
                }

                await UniTask.WaitForSeconds(2);
            }
        }

        private void OnReset()
        {
            foreach (var (_, characterStats) in GamePlayModel.Characters)
            {
                SharedGameObjectPool.Return(characterStats.Transform.gameObject);
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